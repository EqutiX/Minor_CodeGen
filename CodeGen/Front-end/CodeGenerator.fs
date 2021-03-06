﻿module CodeGenerator

open TypeInference
open System
open System.Reflection
open System.IO
open Utilities
open ParserMonad
open BasicExpression
open TypeDefinition
open ConcreteExpressionParserPrelude
open ConcreteExpressionParser
open CodeGen

type TypedExpression   = BasicExpression<Keyword, Var, Literal, Position, Type>
type UntypedExpression = BasicExpression<Keyword, Var, Literal, Position, unit>

let cleanupWithoutDot (s:string) =
  match s with
  | "do" -> "_Do"
  | "is" -> "_Is"
  | "as" -> "_As"
  | "if" -> "_If"
  | "while" -> "_While"
  | "then" -> "_Then"
  | "else" -> "_Else"
  | "for" -> "_For"
  | "true" -> "_True"
  | "false" -> "_False"
  | "with" -> "_With"
  | "in" -> "_In"
  | _ ->
    s
     .Replace("8==D", "_Penis")
     .Replace("c==8", "_ReversePenis")
     .Replace(":=", "_DefinedAs")
     .Replace("->", "_Arrow")
     .Replace("<-", "_LeftArrow")
     .Replace("\'", "_Prime")
     .Replace("\\", "_opSlash")
     .Replace("^", "_opHat")
     .Replace("&&", "_opAnd")
     .Replace("||", "_opOr")
     .Replace("&", "_opBitwiseAnd")
     .Replace("|", "_opBitwiseOr")
     .Replace("$", "_opDollar")
     .Replace("!", "_opBang")
     .Replace("?", "_opQuestion")   
     .Replace("*", "_opMultiplication")
     .Replace("+", "_opAddition")
     .Replace("-", "_opSubtraction")
     .Replace("/", "_opDivision")
     .Replace("|", "_opVBar")
     .Replace(",", "_Comma")
     .Replace(";", "_Semicolon")
     .Replace(":", "_Colon")
     .Replace(">", "_opGreaterThan")
     .Replace("<", "_opLessThan")
     .Replace("=", "_opEquals")
     .Replace("@", "_opAt")
     .Replace("++", "_opListAdd")
     .Replace("[", "<") // keep last or > becomes opGreaterThan
     .Replace("]", ">") // see previous comment

let (!) (s:string) = (cleanupWithoutDot s).Replace(".", "_opDot")

let rec getTypeName (ctxt:ConcreteExpressionContext) k ti = 
  let kDesc = ctxt.CustomKeywordsMap.[k]
  let kArgs = kDesc.GenericArguments
  match kArgs with
  | [] -> !k
  | _ ->
    let kType = kDesc.Type
    let bindings = Map.empty |> ref
    let rec cotraverse original given =
      match original, given with
      | TypeConstant _, TypeConstant _ -> ()
      | TypeVariable(v), _ ->
        bindings := bindings.Value |> Map.add v given
      | ConstructedType(h1,args1), ConstructedType(h2,args2) ->
        cotraverse h1 h2
        for a1,a2 in Seq.zip args1 args2 do
          cotraverse a1 a2
      | TypeAbstraction(a1,b1), TypeAbstraction(a2,b2) ->
        cotraverse a1 a2
        cotraverse b1 b2
      | _ -> failwithf "Cannot co-traverse types %A and %A" original given
    do cotraverse kType ti
    let bindings = bindings.Value
    let res = sprintf "%s<%s>" !k ([ for a in kDesc.GenericArguments -> bindings.[a].CSharpString cleanupWithoutDot ] |> Seq.reduce (fun x y -> x + ", " + y))
    res


let inline (++) (s:#seq<string>) (d:int) = 
  let bs = [ for i in [1..d] -> " " ] |> Seq.fold (+) ""
  s |> Seq.map (fun x -> bs + x + "\n") |> Seq.fold (+) ""

let escape (s:string) = 
  s.Replace("\\", "\\\\")


type Parameter = 
  {
    Name    : string
    IsLeft  : bool
    Type    : TypeDefinition.Type
  }

(*
    Since different scopes can have different implementations of Run(), Path is
    necessary to keep track of the scope.

    Path provides the ParentCall member functions, which calls the Run() of
    the parent.
*)
type Path = Path of List<int>
  with
    override this.ToString() = 
      match this with Path(p) -> p |> List.rev |> Seq.map (fun i -> string i + "_") |> Seq.fold (+) ""
    member this.Tail =
      match this with 
      | Path(p::ps) -> Path ps
      | _ -> failwith "Cannot reduce empty path"
    member this.ParentCall multeplicity =
      match this with
      | Path([]) -> ""
      | Path(p::ps) -> 
        match multeplicity with
        | KeywordMulteplicity.Single ->
          sprintf "var p = Run%s(); return p;" (Path(ps).ToString())
        | KeywordMulteplicity.Multiple -> 
          sprintf "foreach(var p in Run%s()) yield return p;" (Path(ps).ToString())
    member this.DirectParentCall =
      match this with
      | Path([]) -> ""
      | Path(p::ps) -> sprintf "return Run%s();" (Path(ps).ToString())
    member this.StaticParentCall multeplicity parameters =
      match this with
      | Path([]) -> ""
      | Path(p::ps) -> 
        let paramsWithoutType = String.concat ", " (parameters |> Seq.map (fun x -> x.Name))
        match multeplicity with
        | KeywordMulteplicity.Single ->
          sprintf "var p = StaticRun%s(%s); return p;" (Path(ps).ToString()) paramsWithoutType
        | KeywordMulteplicity.Multiple -> 
          sprintf "foreach(var p in StaticRun%s(%s)) yield return p;" (Path(ps).ToString()) paramsWithoutType
    member this.DirectStaticParentCall parameters =
      match this with
      | Path([]) -> ""
      | Path(p::ps) -> 
        let paramsWithoutType = String.concat ", " (parameters |> Seq.map (fun x -> x.Name))
        sprintf "return StaticRun%s(%s);" (Path(ps).ToString()) paramsWithoutType

(*
    Instruction represents the different code-snippets that can be generated in
    the Run functions by generateInstruction.
*)
type Instruction = 
    Var of name : string * expr : string
  | VarAs of name : string * expr : string * as_type : string
  | CheckNull of var_name : string
  | CustomCheck of condition : string
  | Call of var_name : string * tmp_var_name : string * expr:TypedExpression * path : Path
  | Iterate of var_name : string * tmp_var_name : string * expr:TypedExpression * path : Path
  | Compare of comparison : Keyword * expr1:TypedExpression * expr2:TypedExpression
  | Inline of e:TypedExpression
  | Yield of expr:TypedExpression
  | Return of expr:TypedExpression

(*
    createElement is responsible for generating the Create() functions.
*)
let rec generate_inline =
  function
  | Keyword(Custom k,_,_) ->
    sprintf "%s" k
  | Application(_,[],_,_) ->
    ""
  | Application(Regular,(Keyword(Custom k,_,_)) :: [],_,_) -> 
    sprintf "(%s)" k
  | Application(Square,(Keyword(Custom k, _,_)) :: l :: r :: [],pos,_) when ConcreteExpressionContext.CSharp.CustomKeywordsMap.[k].LeftArguments.Length = 1 ->
    sprintf "<%s%s%s>" (l |> generate_inline) k (r |> generate_inline)
  | Application(Regular,(Keyword(Custom k,_,_)) :: l :: r :: [],pos,_) when ConcreteExpressionContext.CSharp.CustomKeywordsMap.[k].LeftArguments.Length = 1 ->
    sprintf "(%s%s%s)" (l |> generate_inline) k (r |> generate_inline)
  | Extension(v:Var,_,_) ->
    sprintf "%s" !v.Name
  | Application(Angle, (Keyword(Inlined, _, _)) :: args, di, ti) ->
    let res = Application(Implicit, args, di, ti) |> generate_inline
    res
  | Application(Regular, a :: [], _, _) ->
    sprintf "(%s)" (a |> generate_inline)
  | Application(Regular, arg :: args, _, _) ->
    let pars = [ for x in args -> x |> generate_inline ] |> Seq.reduce (fun s x -> sprintf "%s%s" s x)
    sprintf "%s(%s)" (arg |> generate_inline) pars
  | Application(Angle, arg :: args, pos, ti) ->
    let res = Application(Implicit, arg :: args, pos, ti) |> generate_inline
    res
  | Application(Implicit,(Keyword(Custom k, _, _)) :: [],_,_) -> 
    sprintf "%s" k
  | Application(Implicit,(Keyword(Custom k, _, _)) :: l :: r :: [], pos, _) when ConcreteExpressionContext.CSharp.CustomKeywordsMap.[k].LeftArguments.Length = 1 ->
    sprintf "%s%s%s" (l |> generate_inline) k (r |> generate_inline)
  | Application(Implicit, arg :: [], _, _) ->
    arg |> generate_inline
  | Application(Implicit, arg :: args, _, _) ->
    let res = [ for x in arg :: args -> x |> generate_inline ] |> Seq.reduce (fun s x -> sprintf "%s%s" s x)
    res
  | Imported(l,di,_) ->
    l.ToString()
  | i -> 
    failwithf "Code generation error @ %A" i.DebugInformation
  
let rec createElementInner (ctxt:ConcreteExpressionContext) (expectedType:Type) (e:TypedExpression) = 
    match e with
    | Keyword(Custom k, _, ti) 
    | Application(Regular,(Keyword(Custom k, _, ti)) :: [], _, _)
    | Application(Implicit,(Keyword(Custom k, _, ti)) :: [], _, _) -> 
      sprintf "%s.Create()" (getTypeName ctxt k ti), []
    | Application(Regular,(Keyword(Custom k, _, ti)) :: es, pos, _)
    | Application(Implicit,(Keyword(Custom k, _, ti)) :: es, pos, _) ->
      let actualKeyword = ctxt.CustomKeywordsMap.[k]
      if es.Length = (actualKeyword.LeftArguments@actualKeyword.RightArguments).Length then
        let args,cargs = es |> Seq.mapi (fun i e -> createElementInner ctxt (actualKeyword.LeftArguments@actualKeyword.RightArguments).[i] e) |> Seq.reduce (fun (s,cs) (x,cx) -> sprintf "%s, %s" s x, cs @ cx)
        //do printfn "Inner creation of %A with expectedType %A" (k, args) expectedType
        sprintf "%s.Create(%s)" (getTypeName ctxt k ti) args, cargs
      else
        failwithf "Invalid number of keyword arguments @ %A" pos
    | Extension(v:Var, _, ti) ->
      match expectedType with
      | TypeConstant(t, TypeConstantDescriptor.NativeValue) ->
        sprintf "%s" !v.Name, []
      | _ ->
        match ti, expectedType with
        | TypeConstant(found,_), TypeConstant(expected,_) ->
          if found = expected || ctxt.Inherits found expected then
            sprintf "%s" !v.Name, []
          else
            sprintf "%s" !v.Name, []
        | _ ->
          let t = Keyword.ArgumentCSharpStyle expectedType cleanupWithoutDot
          sprintf "%s" !v.Name, []
    | Application(Angle,e::[],pos, _) ->
      let res = generate_inline e
      sprintf "%s" res, []
    | Application(Angle,e::es,pos,typeInfo) ->
      let res = generate_inline (Application(Implicit,e::es,pos,typeInfo))
      sprintf "%s" res, []
    | Application(Regular,e::[],pos,typeInfo) ->
      let res, conds = createElementInner ctxt expectedType e
      sprintf "(%s)" res, conds
    | Application(b,e::es,pos,typeInfo) ->
      failwithf "Code generation error @ %A" pos
    | Application(b,[],pos,typeInfo) ->
      failwithf "Code generation error @ %A" pos
    | Imported(l,pos,typeInfo) -> l.ToString(), []
    | Keyword(k, pos, typeInfo) -> 
      failwithf "Unexpected keyword %A @ %A cannot be matched" k pos

let createElement (ctxt:ConcreteExpressionContext) (e:TypedExpression) = 
  match e with
  | Keyword(Custom k, _,ti) 
  | Application(Regular,(Keyword(Custom k, _,ti)) :: [],_,_)
  | Application(Implicit,(Keyword(Custom k, _,ti)) :: [],_,_) -> 
    sprintf "%s.Create()" (getTypeName ctxt k ti), []
  | Application(Regular,(Keyword(Custom k, _,ti)) :: es,pos,_)
  | Application(Implicit,(Keyword(Custom k, _,ti)) :: es,pos,_) ->
    let actualKeyword = ctxt.CustomKeywordsMap.[k]
    let argumentTypes = actualKeyword.LeftArguments @ actualKeyword.RightArguments
    let args,cargs = es |> Seq.mapi (fun i e -> createElementInner ctxt argumentTypes.[i] e) |> Seq.reduce (fun (s,cs) (x,cx) -> sprintf "%s, %s" s x, cs @ cx)
    let trimmedArgs = 
      if es |> List.length = 1 && args.[0] = '(' && args.[args.Length - 1] = ')' then
        args.Substring(1, args.Length - 2)
      else
        args
    let res = sprintf "%s.Create(%s)" (getTypeName ctxt k ti) trimmedArgs, cargs
    res
  | Application(Angle,e::es,pos,ti) ->
    let res = generate_inline (Application(Regular,e::es,pos,ti))
    sprintf "%s" res, []
  | Extension(v:Var,_,_) ->
    sprintf "%s" !v.Name, []
  | Application(b,e::es,pos,ti) ->
    failwithf "Code generation error @ %A" pos
  | Application(b,[],pos,_) ->
    failwithf "Code generation error @ %A" pos
  | Imported(l,pos,_) -> l.ToString(), []
  | Keyword(k,pos,_) -> 
    failwithf "Non-custom keyword %A @ %A cannot be matched" k pos
  
let createStaticRun (ctxt:ConcreteExpressionContext) (path:Path) (e:TypedExpression) = 
  match e with
  | Keyword(Custom k, _,ti) 
  | Application(Regular,(Keyword(Custom k, _,ti)) :: [],_,_)
  | Application(Implicit,(Keyword(Custom k, _,ti)) :: [],_,_) -> 
    sprintf "%s.StaticRun%s()" (getTypeName ctxt k ti) (path.ToString()), []
  | Application(Regular,(Keyword(Custom k, _,ti)) :: es,pos,_)
  | Application(Implicit,(Keyword(Custom k, _,ti)) :: es,pos,_) ->
    let actualKeyword = ctxt.CustomKeywordsMap.[k]
    let args,cargs = es |> Seq.mapi (fun i e -> createElementInner ctxt (actualKeyword.LeftArguments@actualKeyword.RightArguments).[i] e) |> Seq.reduce (fun (s,cs) (x,cx) -> sprintf "%s, %s" s x, cs @ cx)
    let trimmedArgs = 
      if es |> List.length = 1 && args.[0] = '(' && args.[args.Length - 1] = ')' then
        args.Substring(1, args.Length - 2)
      else
        args
    let res = sprintf "%s.StaticRun%s(%s)" (getTypeName ctxt k ti) (path.ToString()) trimmedArgs, cargs
    res
  | Application(b,e::es,pos,ti) ->
    failwithf "Code generation error @ %A" pos
  | Application(b,[],pos,_) ->
    failwithf "Code generation error @ %A" pos
  | Imported(l,pos,_) -> l.ToString(), []
  | Keyword(k,pos,_) -> 
    failwithf "Non-custom keyword %A @ %A cannot be matched" k pos
  | _ -> failwithf "Unexpected expression %A when creating StaticRun method" e

(*
    generateInstructions generates the body of a Run function from a list of
    'Instruction's.
*)
let rec generateInstructions (debugPosition:Position) (originalFilePath:string) (ctxt:ConcreteExpressionContext) = 
  function 
  | [] -> ""
  | x :: xs ->
    let newLine = 
      if CompilerSwitches.generateLineDirectives then sprintf "\n #line %d \"%s\"\n" debugPosition.Line debugPosition.File
      else "\n"
    match x with
    | Var(name, expr) -> 
      let res = sprintf "var %s = %s; %s" !name expr (generateInstructions debugPosition originalFilePath ctxt xs)
      res
    | VarAs(name, expr, as_type) ->
      sprintf "var %s = %s as %s; %s" !name expr as_type (generateInstructions debugPosition originalFilePath  ctxt xs)
    | CheckNull(var_name) ->
      let printed_string = "not null: "+(!var_name)
      sprintf "%sif (%s != null) { \n %s }" newLine !var_name (*printed_string*) (generateInstructions debugPosition originalFilePath  ctxt xs)
    | Compare(comparison, expr1, expr2) ->
      let newElement1, creationConstraints1 = createElement ctxt expr1
      let newElement2, creationConstraints2 = createElement ctxt expr2
      let creationConstraints = creationConstraints1 @ creationConstraints2
      let preComparison, inlineComparison, postComparison = 
        match comparison with 
        | Equals -> "", ".Equals(", ")" 
        | NotEquals -> "!", ".Equals(", ")" 
        | GreaterThan -> "", ">", "" 
        | GreaterOrEqual -> "", ">=", "" 
        | SmallerThan -> "", "<", "" 
        | SmallerOrEqual -> "", "<=", "" 
        | Divisible -> "", "%", "== 0" 
        | NotDivisible -> "", "%", "!= 0" 
        | _ -> failwith "Unsupported comparison operator %s @ %A" comparison expr1.DebugInformation
      let res =
        if creationConstraints.IsEmpty |> not then
          let creationConstraints = creationConstraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
          sprintf "%sif(%s) { %sif(%s%s%s%s%s) { %s } }" newLine creationConstraints newLine preComparison newElement1 inlineComparison newElement2 postComparison (generateInstructions debugPosition originalFilePath  ctxt xs)
        else 
          sprintf "%sif(%s%s%s%s%s) { %s }" newLine preComparison newElement1 inlineComparison newElement2 postComparison (generateInstructions debugPosition originalFilePath  ctxt xs)
      res
    | Iterate(var_name, tmp_var_name, expr, path) ->
      if CompilerSwitches.combineCreateFor && CompilerSwitches.generateStaticRun then
        let newElement, creationConstraints = createStaticRun ctxt path expr
        if creationConstraints.IsEmpty |> not then
          let creationConstraints = creationConstraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
          sprintf "%sif(%s) { %svar %s = %s;%sforeach (var %s in %s) { %s } }" newLine creationConstraints newLine !tmp_var_name newElement newLine !var_name !tmp_var_name (generateInstructions debugPosition originalFilePath  ctxt xs)
        else 
          sprintf "%svar %s = %s;%sforeach (var %s in %s) { %s }" newLine !tmp_var_name newElement newLine !var_name !tmp_var_name (generateInstructions debugPosition originalFilePath ctxt xs) 
      else
        let newElement, creationConstraints = createElement ctxt expr
        if creationConstraints.IsEmpty |> not then
          let creationConstraints = creationConstraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
          sprintf "%sif(%s) { %svar %s = %s;%sforeach (var %s in %s.Run%s()) { %s } }" newLine creationConstraints newLine !tmp_var_name newElement newLine !var_name !tmp_var_name (path.ToString()) (generateInstructions debugPosition originalFilePath  ctxt xs)
        else 
          sprintf "%svar %s = %s;%sforeach (var %s in %s.Run%s()) { %s }" newLine !tmp_var_name newElement newLine !var_name !tmp_var_name (path.ToString()) (generateInstructions debugPosition originalFilePath ctxt xs)
    | Call(var_name, tmp_var_name, expr, path) ->
//      let optionalStaticRun = if CompilerSwitches.combineCreateFor && CompilerSwitches.generateStaticRun then "" else sprintf ".Run%s()" (path.ToString())
      let optionalTabIn  = if CompilerSwitches.printExpressionTree then "System.Console.Write(\"<ul>\");" else ""
      let optionalTabOut = if CompilerSwitches.printExpressionTree then "System.Console.Write(\"</ul>\");" else ""
      let newElement, creationConstraints = 
        if CompilerSwitches.combineCreateFor && CompilerSwitches.generateStaticRun then
          createStaticRun ctxt path expr
        else
          createElement ctxt expr          
      if creationConstraints.IsEmpty |> not then
        let creationConstraints = creationConstraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
        let res = sprintf "%s%sif(%s) { %svar %s = %s;%s\nvar %s = %s%s;\n%s\n%s }" 
                      newLine optionalTabIn creationConstraints newLine !tmp_var_name newElement newLine !var_name !tmp_var_name 
                      "" optionalTabOut (generateInstructions debugPosition originalFilePath  ctxt xs) 
        res
      else 
        let res = sprintf "%s%svar %s = %s;%s\nvar %s = %s%s;\n%s\n%s" 
                      newLine optionalTabIn !tmp_var_name newElement newLine !var_name !tmp_var_name 
                      "" optionalTabOut (generateInstructions debugPosition originalFilePath ctxt xs) 
        res

    | CustomCheck(condition) ->
      sprintf "%sif (%s) { %s }" newLine condition (generateInstructions debugPosition originalFilePath  ctxt xs)
    | Inline(c) -> 
      sprintf "%s%s;%s%s" newLine (generate_inline c) newLine (generateInstructions debugPosition originalFilePath  ctxt xs)
    | Yield(expr) ->
      let newElement, creationConstraints = createElement ctxt expr
      if creationConstraints.IsEmpty |> not then
        let creationConstraints = creationConstraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
        sprintf "%sif(%s) { %svar result = %s;%syield return result; %s }" newLine creationConstraints newLine newElement newLine (generateInstructions debugPosition originalFilePath ctxt xs)
      else
        sprintf "%svar result = %s;%syield return result; %s" newLine newElement newLine (generateInstructions debugPosition originalFilePath ctxt xs)
    | Return(expr) ->
      let newElement, creationConstraints = createElement ctxt expr
      if creationConstraints.IsEmpty |> not then
        let creationConstraints = creationConstraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
        sprintf "%sif(%s) { %svar result = %s;%s return result; %s }" newLine creationConstraints newLine newElement newLine (generateInstructions debugPosition originalFilePath ctxt xs)
      else
        sprintf "%svar result = %s;%s return result; %s" newLine newElement newLine (generateInstructions debugPosition originalFilePath ctxt xs)

(*
    matchCast is responsible for matching expressions
*)
let rec matchCast (tmp_id:int) (e:TypedExpression) (self:string) (prefix:List<Instruction>) 
                  (ctxt:ConcreteExpressionContext) (typeConstraint) =
  match e with
  | Keyword(Custom k, _, ti) -> 
    if self <> "this" then
      prefix @ 
      [
        VarAs(sprintf "tmp_%d" tmp_id, self, getTypeName ctxt k ti)
        CheckNull(sprintf "tmp_%d" tmp_id)
      ], tmp_id+1
    else
      if CompilerSwitches.generateStaticRun
      then
        prefix @ [], tmp_id
      else
        prefix @
        [
          VarAs(sprintf "tmp_%d" tmp_id, self, getTypeName ctxt k ti)
        ], tmp_id+1
        
  | Extension(v:Var, _, _) ->
      prefix @
      [
        Var(v.Name, self)
      ], tmp_id
  | Application(Angle,es,pos,ti) ->
    let output,tmp_id = 
      let expectedSelf = generate_inline(Application(Angle,es,pos,ti))
      prefix @
      [
        CustomCheck(sprintf "%s == %s" self expectedSelf)
      ], tmp_id
    // es_i -> self . P_i
    output, tmp_id
  | Application(b,(Keyword(Custom k, _, k_ti)) :: es,pos,e_ti) ->
    let output,self,tmp_id = 
      if self <> "this" then
        prefix @
        [
          VarAs(sprintf "tmp_%d" tmp_id, self, getTypeName ctxt k k_ti)
          CheckNull(sprintf "tmp_%d" tmp_id)
        ], sprintf "tmp_%d" tmp_id, tmp_id+1
      else
        if CompilerSwitches.generateStaticRun then
          prefix @ [], "this", tmp_id
        else
          prefix @
          [
            Var(sprintf "tmp_%d" tmp_id, self)
          ], sprintf "tmp_%d" tmp_id, tmp_id+1
    // es_i -> self . P_i
    let mutable output = output
    let mutable tmp_id = tmp_id
    let actualKeyword = ctxt.CustomKeywordsMap.[k]
    let keywordArgumentConstraints = actualKeyword.LeftArguments @ actualKeyword.RightArguments
    for e_constraint,(e,i) in es |> List.mapi (fun i e -> e,(i+1)) |> Seq.zip keywordArgumentConstraints do
      let varAccess = 
        if self="this" && CompilerSwitches.generateStaticRun
        then sprintf "P%d" i
        else sprintf "%s.P%d" self i
      let newOutput, newTempId = matchCast tmp_id e varAccess output ctxt (Some e_constraint)
      output <- newOutput
      tmp_id <- newTempId
    output, tmp_id
  | Application(Regular,e::[],pos,_) ->
    matchCast tmp_id e self prefix ctxt None
  | Application(Square,e::es,pos,_) ->
    prefix, tmp_id
  | Application(b,e::es,pos,_) ->
    failwithf "Code generator error @ %A" pos
  | Application(b,[],pos,_) ->
    failwithf "Code generator error @ %A" pos
  | Imported(l,pos,_) ->
    let cond = sprintf "%s == %s" self (l.ToString())
    prefix @
    [
      CustomCheck(cond)
    ], tmp_id
  | Keyword(k,pos,ti) -> 
    failwithf "Unexpected keyword %A @ %A" k pos

(*
    a Rule is a line division (-------------) and it's inputs, outputs and clauses
*)
type Rule = {
  Position   : Position
  Input      : TypedExpression
  Output     : TypedExpression
  Clauses    : List<Keyword * TypedExpression * TypedExpression>
  Path       : Path
  HasScope   : bool
} with
    member r.ToString (ctxt:ConcreteExpressionContext,originalFilePath) =
      let path = 
        if r.HasScope then
          r.Path
        else
          r.Path.Tail
      let i,tmp_id = matchCast 0 r.Input "this" [] ctxt None
      let mutable o = []
      let mutable tmp_id = tmp_id
      for k,c_i,c_o in r.Clauses do
        match k with
        | DoubleArrow ->
          let c_i_keyword_name = extractLeadingKeyword c_i
          let c_i_keyword = ctxt.CustomKeywordsMap.[c_i_keyword_name]
          match c_i_keyword.Multeplicity with
          | KeywordMulteplicity.Single ->
            o <- o @ [Call(sprintf "tmp_%d" tmp_id, sprintf "tmp_%d" (tmp_id+1), c_i, path)]
          | KeywordMulteplicity.Multiple ->
            o <- o @ [Iterate(sprintf "tmp_%d" tmp_id, sprintf "tmp_%d" (tmp_id+1), c_i, path)]
          let o',tmp_id' = matchCast (tmp_id+2) c_o (sprintf "tmp_%d" tmp_id) [] ctxt None
          o <- o @ o'
          tmp_id <- tmp_id'
        | Equals | NotEquals | Divisible | NotDivisible | GreaterThan | GreaterOrEqual | SmallerThan | SmallerOrEqual -> 
          o <- o @ [Compare(k, c_i, c_o)]
        | DefinedAs -> 
          match c_i with
          | Extension(iVar, _, _) ->
            let oExpr,constraints = createElement ctxt c_o
            if constraints.IsEmpty |> not then
              o <- o @ [CustomCheck(constraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x))]
            o <- o @ [Var(iVar.Name, oExpr)]
          | _ -> 
            let iVar = c_i.ToStringCompact
            let oExpr,constraints = createElement ctxt c_o
            if constraints.IsEmpty |> not then
              o <- o @ [CustomCheck(constraints |> Seq.reduce (fun s x -> sprintf "%s && %s" s x))]
            o <- o @ [Var(iVar, oExpr)]
        | Inlined ->
            o <- o @ [Inline(c_i)]
        | _ -> failwithf "Unexpected operator %A @ %A" k c_i.DebugInformation

      let i_keyword_name = extractLeadingKeyword r.Input
      let i_keyword = ctxt.CustomKeywordsMap.[i_keyword_name]
      match i_keyword.Multeplicity with
      | KeywordMulteplicity.Single ->
        o <- i @ o @ [Return r.Output]
      | KeywordMulteplicity.Multiple ->
        o <- i @ o @ [Yield r.Output]
      if CompilerSwitches.generateLineDirectives then
        sprintf "\n { \n #line %d \"%s\"\n%s\n } \n" r.Position.Line r.Position.File (generateInstructions r.Position originalFilePath ctxt o)
      else
        sprintf "\n { \n %s\n } \n" (generateInstructions r.Position originalFilePath ctxt o)

type Method = {
  Keyword    : ParsedKeyword<Keyword, Var, Literal, Position, unit>
  Rules      : ResizeArray<Rule>
  Path       : Path
} with
    member m.ToString(ctxt:ConcreteExpressionContext,originalFilePath,parameters,returnType) =
      let path = m.Path.ToString()
      let paramsWithType    = String.concat ", " (parameters |> Seq.map (fun x -> sprintf "%s %s" (Keyword.ArgumentCSharpStyle x.Type cleanupWithoutDot) x.Name))
      let paramsWithoutType = String.concat ", " (parameters |> Seq.map (fun x -> x.Name))
      let body = ((m.Rules |> Seq.map (fun r -> r.ToString(ctxt,originalFilePath))) ++ 2)
      //let self_constructor = sprintf "new %s(%s)" !m.Keyword.Name paramsWithoutType
      let final_throw =
        match m.Keyword.Multeplicity with
        | KeywordMulteplicity.Single ->
          //sprintf "\nthrow new System.Exception(\"Error evaluating: \" + %s.ToString() + \" no result returned.\");" self_constructor
          sprintf "\nthrow new System.Exception(\"Error evaluating: %s. No result returned.\");" !m.Keyword.Name
        | KeywordMulteplicity.Multiple -> 
          ""
      let parent_call = if CompilerSwitches.generateStaticRun then m.Path.StaticParentCall m.Keyword.Multeplicity parameters else m.Path.ParentCall m.Keyword.Multeplicity
      let function_trace = "" // TODO: "add relevant compiler switches" sprintf "System.Console.WriteLine(\"<li>\"+%s.ToString()+\"</li>\");" self_constructor
      sprintf "public static %s StaticRun%s(%s) { %s %s %s %s }\npublic %s Run%s() { return StaticRun%s(%s); }" 
                returnType path paramsWithType function_trace body parent_call final_throw returnType path path paramsWithoutType
    member m.ToString(ctxt:ConcreteExpressionContext,originalFilePath,returnType) =
      let path = m.Path.ToString()
      let body = ((m.Rules |> Seq.map (fun r -> r.ToString(ctxt,originalFilePath))) ++ 2)
      sprintf "public IEnumerable<%s> Run%s() { %s %s }" returnType path body (m.Path.ParentCall m.Keyword.Multeplicity)
      
type GeneratedClass = 
  {
    cb                  : ClassBuilder
    Keyword             : ParsedKeyword<Keyword, Var, Literal, Position, unit>
    BasicName           : string
    Interface           : string
    GenericArguments    : List<string>
    Parameters          : ResizeArray<Parameter>
    mutable Methods     : Map<Path, Method>
  } with
      member c.Name =
        if c.GenericArguments.IsEmpty |> not then
          let args = c.GenericArguments |> Seq.reduce (fun s x -> sprintf "%s, %s" s x)
          sprintf "%s<%s>" !c.BasicName args
        else
          !c.BasicName

      member this.MethodPaths = seq{ for x in this.Methods -> x.Key } |> Set.ofSeq
      member c.ToString(all_method_paths:Set<Path>, ctxt:ConcreteExpressionContext, originalFilePath) =
        let genericConstraints = ""
//          if c.GenericArguments.IsEmpty |> not then
//            let args = c.GenericArguments |> Seq.map (fun x -> sprintf "where %s : class" x) |> Seq.reduce (fun s x -> sprintf "%s %s" s x)
//            args
//          else
//            ""
        let parametersWithType, parametersWithoutType = 
          if c.Parameters.Count <> 0 then
            (c.Parameters |> Seq.map (fun x -> sprintf "%s %s" (Keyword.ArgumentCSharpStyle x.Type cleanupWithoutDot) x.Name) |> Seq.reduce (fun s x -> sprintf "%s, %s" s x)),
            (c.Parameters |> Seq.map (fun x -> sprintf "%s" x.Name) |> Seq.reduce (fun s x -> sprintf "%s, %s" s x))
          else
            "", ""
        let cons =
          if c.Parameters.Count <> 0 then
            let args = c.Parameters |> Seq.map (fun x -> sprintf "this.%s = %s;" x.Name x.Name) |> Seq.reduce (fun s x -> sprintf "%s %s" s x)
            sprintf "public %s(%s) {%s}\n" !c.BasicName parametersWithType args
          else
            sprintf "public %s() {}\n" !c.BasicName
        let staticCons =
          if c.Parameters.Count <> 0 then
            let args = c.Parameters |> Seq.map (fun x -> sprintf "%s" x.Name) |> Seq.reduce (fun s x -> sprintf "%s, %s" s x)
            sprintf "public static %s Create(%s) { return new %s(%s); }\n" c.Name parametersWithType c.Name args
          else
            sprintf "public static %s Create() { return new %s(); }\n" c.Name c.Name
        let cons = cons + staticCons
        let parameters =
          let res = c.Parameters |> Seq.map (fun p -> sprintf "public %s %s;\n" (Keyword.ArgumentCSharpStyle p.Type cleanupWithoutDot) p.Name) |> Seq.fold (+) "" 
          res
        let to_string =
          if c.Parameters.Count > 0 then
            let printParameter (p:Parameter) = 
              match p.Type with
              | TypeDefinition.TypeConstant(t, TypeDefinition.TypeConstantDescriptor.Defined)
              | TypeDefinition.TypeConstant(t, TypeDefinition.TypeConstantDescriptor.NativeRef)
              | TypeDefinition.TypeVariable(t, _)
              | TypeDefinition.ConstructedType(TypeDefinition.TypeConstant(t, _),_) when t <> "int" && t <> "float" && t <> "double" && t <> "bool" ->
                let currentType =
                  [for entry in ctxt.AssemblyInfo do
                      if t = entry.Value.FilteredFullName || 
                         t = entry.Value.FilteredName then
                            yield entry.Key]
                if currentType.IsEmpty |> not then
                  let interfaces = ctxt.AssemblyInfo.[currentType.Head].Interfaces
                  //Cesco :: I do not understand why reflection does not show IEnumerable among ImmutableDictionary collections even if in the generated code the cast works.
                  //Added special case for that in the "if" condition
                  if interfaces |> Array.exists (fun i -> i = typeof<System.Collections.IEnumerable>) || currentType.Head.Namespace = "System.Collections.Immutable" then
                    sprintf "if (%s is System.Collections.IEnumerable) { res += \"{\"; foreach(var x in %s as System.Collections.IEnumerable) res += x.ToString(); res += \"}\";  } else { res += %s.ToString(); } \n" p.Name p.Name p.Name
                  else
                    sprintf "res += %s.ToString(); \n" p.Name
                else
                  sprintf "res += %s.ToString(); \n" p.Name
                  
              | _ -> 
                sprintf "res += %s.ToString(); \n" p.Name
            let leftParameters = c.Parameters |> Seq.filter (fun x -> x.IsLeft) |> Seq.map (fun x -> printParameter x) |> Seq.fold (+) ""
            let rightParameters = c.Parameters |> Seq.filter (fun x -> x.IsLeft |> not) |> Seq.map (fun x -> printParameter x) |> Seq.fold (+) ""
            sprintf "public override string ToString() {\n var res = \"(\"; \n%s\n res += \" %s \"; %s\n res += \")\";\n return res;\n}\n" leftParameters (escape c.BasicName) rightParameters
          else
            sprintf "public override string ToString() {\nreturn \"%s\";\n}\n" (escape c.BasicName)
        let equals =
          if c.Parameters.Count > 0 then
            let parameters = c.Parameters |> Seq.map (fun x -> sprintf "this.%s.Equals(tmp.%s)" x.Name x.Name) |> Seq.reduce (fun s x -> sprintf "%s && %s" s x)
            sprintf "public override bool Equals(object other) {\n var tmp = other as %s;\n if(tmp != null) return %s; \n else return false; }\n" c.Name parameters
          else
            sprintf "public override bool Equals(object other) {\n return other is %s; \n}\n" c.Name
        let hash =
          sprintf "public override int GetHashCode() {\n return 0; \n}\n"
        let (!) l = l |> List.reduce (fun p n -> p + "," + n)
        match c.Keyword.Kind with
        | KeywordKind.Func returnType ->
          let returnType = Keyword.ArgumentCSharpStyle returnType cleanupWithoutDot
          let returnType = 
            match c.Keyword.Multeplicity with
            | KeywordMulteplicity.Single ->
              returnType
            | KeywordMulteplicity.Multiple ->
              sprintf "IEnumerable<%s>" returnType
          let runImplementation =
            if CompilerSwitches.generateStaticRun
            then ((c.Methods |> Seq.map (fun x -> x.Value.ToString(ctxt,originalFilePath,c.Parameters,returnType))) ++ 2)
            else ((c.Methods |> Seq.map (fun x -> x.Value.ToString(ctxt,originalFilePath,returnType))) ++ 2)
          let missing_methods =
            let missing_paths = all_method_paths - c.MethodPaths
            let self_constructor = sprintf "new %s(%s)" (cleanupWithoutDot c.Keyword.Name) parametersWithoutType
            [
              for p in missing_paths do
              let parent_call = 
                match CompilerSwitches.generateStaticRun,CompilerSwitches.optimizeDirectParentCall with
                | false,false -> p.ParentCall c.Keyword.Multeplicity
                | false,true  -> p.DirectParentCall 
                | true, false -> p.StaticParentCall c.Keyword.Multeplicity c.Parameters
                | true, true  -> p.DirectStaticParentCall c.Parameters
              let parent_or_empty_call =
                if parent_call <> "" then
                  parent_call
                else
                  match c.Keyword.Multeplicity with
                  | KeywordMulteplicity.Single ->
                    sprintf "\nthrow new System.Exception(\"Error evaluating: %s no result returned.\");" c.Keyword.Name
                  | KeywordMulteplicity.Multiple ->
                    "foreach (var p in Enumerable.Range(0,0)) yield return null;"
              let path = p.ToString()
              if CompilerSwitches.generateStaticRun 
              then
                let paramsWithoutType = String.concat ", " (c.Parameters |> Seq.map (fun x -> x.Name))
                let paramsWithType    = String.concat ", " (c.Parameters |> Seq.map (fun x -> sprintf "%s %s" (Keyword.ArgumentCSharpStyle x.Type cleanupWithoutDot) x.Name))
                yield sprintf "public static %s StaticRun%s(%s) { %s }\npublic %s Run%s(){ return StaticRun%s(%s); }\n" 
                                returnType path paramsWithType parent_or_empty_call returnType path path paramsWithoutType
              else
                yield sprintf "public %s Run%s(){ %s }\n" returnType path parent_or_empty_call
            ] |> Seq.fold (+) ""
            // USe CLassbuilder
          sprintf "public class %s : %s %s {\n%s\n%s\n%s\n%s\n%s\n%s\n%s\n}\n\n" 
            c.Name c.Interface genericConstraints parameters cons runImplementation missing_methods to_string equals hash
        | KeywordKind.Data ->
        // USe CLassbuilder
          let cb = new ClassBuilder(c.Name, TypeAttributes.Public, false, c.Interface)
          cb = cb.AddField(parameters, CodeDom.MemberAttributes.Public)
          sprintf "public class %s : %s %s {\n%s\n%s\n%s\n%s\n%s\n}\n\n" c.Name c.Interface genericConstraints parameters cons to_string equals hash

let add_rule inputClass (rule:BasicExpression<_,_,Literal, Position, Unit>) (rule_path:Path) (hasScope:bool) ctxt =
  let method_path = rule_path.Tail
  if inputClass.Methods |> Map.containsKey method_path |> not then
    inputClass.Methods <- inputClass.Methods |> Map.add method_path { Keyword = inputClass.Keyword; Rules = ResizeArray(); Path = method_path }
  match rule with
  | Application(Implicit, Keyword(FractionLine, _, _) :: (Application(Implicit, Keyword(DoubleArrow, _, _) :: input :: output :: [], innerPos, _)) :: clauses, pos, _) ->
    let input, output, clauses = TypeInference.inferTypes input output clauses ctxt
    inputClass.Methods.[method_path].Rules.Add(
      { Position = pos
        Input = input
        Clauses = 
          [ for c in clauses do
              match c with
              | Application(_, Keyword(Inlined, _, _) :: _, clausePos, _) -> yield Inlined, c, c
              | Application(_, Keyword(DoubleArrow, _, _) :: (Application(Angle, _, _, _) as c_i) :: (Extension(_,_,_) as c_o) :: [], clausePos, _) -> yield DefinedAs, c_o, c_i
              | Application(_, Keyword(DoubleArrow, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield DoubleArrow, c_i, c_o
              | Application(_, Keyword(Equals, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield Equals, c_i, c_o
              | Application(_, Keyword(NotEquals, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield NotEquals, c_i, c_o
              | Application(_, Keyword(DefinedAs, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield DefinedAs, c_i, c_o
              | Application(_, Keyword(GreaterThan, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield GreaterThan, c_i, c_o
              | Application(_, Keyword(GreaterOrEqual, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield GreaterOrEqual, c_i, c_o
              | Application(_, Keyword(SmallerThan, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield SmallerThan, c_i, c_o
              | Application(_, Keyword(SmallerOrEqual, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield SmallerOrEqual, c_i, c_o
              | Application(_, Keyword(Divisible, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield Divisible, c_i, c_o
              | Application(_, Keyword(NotDivisible, _, _) :: c_i :: c_o :: [], clausePos, _) -> yield NotDivisible, c_i, c_o
              | _ -> failwithf "Unexpected clause @ %A" c.DebugInformation
          ] 
        Output   = output
        Path     = rule_path
        HasScope = hasScope })
  | _ ->
    failwithf "Cannot extract rule shape @ %A" rule.DebugInformation

let rec process_rules (classes:Map<string,GeneratedClass>) (path:List<int>) (rules:List<BasicExpression<_,_,Literal,Position, Unit>>) ctxt = 
  for rule,i in rules |> Seq.mapi (fun i r -> r,i) do
    let path' = i :: path
    let self,hasScope = 
      match rule with
      | Application(_, Keyword(Nesting, _, _) :: self :: children, pos, _) -> 
        do process_rules classes path' children ctxt
        self,true
      | self -> self,false
    match self with
    | Application(Implicit, Keyword(FractionLine, _, _) :: (Application(Implicit, Keyword(DoubleArrow, _, _) :: input :: output, clausesPos, _)) :: clauses, pos, _) ->
      let inputKeyword = extractLeadingKeyword input
      let inputClass = classes.[inputKeyword]
      do add_rule inputClass self (Path path') hasScope ctxt
    | _ -> failwithf "Malformed rule @ %A" self.DebugInformation
    ()


type Interface = { Name : string; BaseInterfaces : ResizeArray<string> }

type Namespace = { Namespace : string; 
                   Classes : System.Collections.Generic.List<ClassBuilder>;
                   Interfaces : System.Collections.Generic.List<InterfaceBuilder> }

//Execute
let generateCode (originalFilePath:string) (program_name:string)
                 (rules:UntypedExpression)
                 (program:UntypedExpression)
                 (ctxt:ConcreteExpressionContext) =
  
  match rules with
  | Application(Implicit, Keyword(Sequence, _, _) :: rules, pos, _) ->
    let mutable classes = Map.empty
    let mutable inheritanceRelationships = Map.empty
    for c,a in ctxt.AllInheritanceRelationships do
      match inheritanceRelationships |> Map.tryFind c with
      | Some i -> i.BaseInterfaces.Add a
      | None -> inheritanceRelationships <- inheritanceRelationships |> Map.add c { Name = c; BaseInterfaces = ResizeArray([a]) }
    for keyword in ctxt.CustomKeywords do
      let newClass = { GeneratedClass.cb        = new ClassBuilder(keyword.Name, TypeAttributes.Public, false, "")
                       GeneratedClass.Keyword   = keyword
                       GeneratedClass.BasicName = keyword.Name
                       GeneratedClass.GenericArguments = keyword.GenericArguments |> List.map fst
                       GeneratedClass.Interface = Keyword.ArgumentCSharpStyle keyword.BaseType cleanupWithoutDot
                       GeneratedClass.Parameters = ResizeArray()
                       GeneratedClass.Methods = Map.empty }
      for t,i in keyword.LeftArguments |> Seq.mapi (fun i p -> p,i+1) do
        newClass.Parameters.Add({ Name = sprintf "P%d" i; IsLeft = true; Type = t })
      for t,i in keyword.RightArguments |> Seq.mapi (fun i p -> p,i+1) do
        newClass.Parameters.Add({ Name = sprintf "P%d" (i + keyword.LeftArguments.Length); IsLeft = false; Type = t })
      classes <- classes.Add(keyword.Name,newClass)

    do process_rules classes [] rules ctxt

    let programTyped,_,_ = TypeInference.inferTypes program (Extension({ Name = "___tmp" }, Position.Zero, ())) [] ctxt

    let classes = classes
    let extensions = @"public static class Extensions { public static V GetKey<T, V>(this System.Collections.Immutable.ImmutableDictionary<T, V> self, T key) { return self[key]; } }"
    let interfaces = [ for k in ctxt.CustomKeywords -> Keyword.ArgumentCSharpStyle k.BaseType cleanupWithoutDot ] |> Seq.distinct |> Seq.toList 
    let inheritanceRelationships = inheritanceRelationships
    let interfacesCode = 
      [
        for i in interfaces do
          match inheritanceRelationships |> Map.tryFind (i) with
          | Some ir ->
            let explicitInterfaces = ir.BaseInterfaces
            //nameSpace.Interfaces.Add( new InterfaceBuilder(i,TypeAttributes.Public))
            yield sprintf "public interface %s : %s {}\n" i (explicitInterfaces |> Seq.reduce (fun s x -> s + ", " + x))
          | _ ->
            //nameSpace.Interfaces.Add( new InterfaceBuilder(i,TypeAttributes.Public))
            yield sprintf "public interface %s {}\n" i
      ] |> Seq.fold (+) ""
    let all_method_paths =
      seq{
        for c in classes -> c.Value.MethodPaths
      } |> Seq.reduce (+)
    let imports = 
        if ctxt.ImportedModules.Length > 0 then
            (ctxt.ImportedModules
                    |> List.map (fun x -> sprintf "using %s;\n" x)
                    |> List.reduce (fun x y -> x + y))
        else
            ""
    let prelude = sprintf "using System.Collections.Generic;\nusing System.Linq;\nnamespace %s {\n %s\n" (program_name.Replace(" ", "_")) extensions
    let programKeyword = ctxt.CustomKeywordsMap.[programTyped |> extractLeadingKeyword]
    let main = 
      let printedHead = "<head><link rel=\\\"stylesheet\\\" type=\\\"text/css\\\" href=\\\"style.css\\\"></head>\\n<ul class=\\\"tree\\\">\\n"
      let printedTail = "\\n</ul>"
      let OptionalPrintHead = if CompilerSwitches.printExpressionTree then sprintf "System.Console.WriteLine(\"%s\");" printedHead else ""
      let OptionalPrintTail = if CompilerSwitches.printExpressionTree then sprintf "System.Console.WriteLine(\"%s\");" printedTail else ""

      match programKeyword.Multeplicity with
      | KeywordMulteplicity.Single ->
        
        // Create the class
        let cb = new ClassBuilder("EntryPoint", TypeAttributes.Public, false)
        
        // Create the input parameter of the function
        let parameterItems = [|new ParameterItem( Name = "s", Type = typeof<string>)|]
        
        // Create the return statement
        let returnStatement = new Statements.ReturnStatementLine()
        returnStatement.Expressions.Add( 0, new Expressions.PrimitiveExpressionLine( Value = true ) )

        // Create the execute statements (like WriteLine)
        let statementLine = new Statements.ExpressionStatementLine()
        statementLine.Expressions.Add( 0, new Expressions.MethodInvokeExpressionLine
                            ( MethodName = "WriteLine", 
                              TargetObject = new Expressions.TypeReferenceExpressionLine( Type = "System.Console" ), 
                              Parameters = [|new Expressions.VariableReferenceExpressionLine( VariableName = "s" )|] ) )

        //static public object Run(bool printInput)
        let parameterItems2 = [|new ParameterItem( Name = "printInput", Type = typeof<bool>)|]
        
        // TODO: This needs to have more info of the FUNCTION createElement
        // var p = %s; (classname.function)
        let statementLine2 = new Statements.VariableDeclarationStatementLine( Name = "p", Type = typeof<System.Object> )
        statementLine2.Expressions.Add( 0, new Expressions.ObjectCreateExpressionLine( CreateType = createElement.GetType().ToString(), Parameters = [||]) )

        // if(printInput) System.Console.WriteLine(p.ToString());
        let statementLine3 = new Statements.ConditionStatementLine()
        statementLine3.Expressions.Add(0, new Expressions.VariableReferenceExpressionLine( VariableName = "printInput" ) )
        let trueStatement = new Statements.ExpressionStatementLine()
        trueStatement.Expressions.Add(0, new Expressions.MethodInvokeExpressionLine
                            ( MethodName = "WriteLine",
                              TargetObject = new Expressions.TypeReferenceExpressionLine( Type = "System.Console" ),
                              Parameters = [|new Expressions.VariableReferenceExpressionLine( VariableName = "p" )|] ) )
        statementLine3.TrueStatementLines <- [|trueStatement|]

        // Create object result = p.Run();
        let statementLine4 = new Statements.VariableDeclarationStatementLine( Name = "result", Type = typeof<System.Object> )
        statementLine4.Expressions.Add( 0, new Expressions.MethodInvokeExpressionLine
                            ( MethodName = "Run", 
                              TargetObject = new Expressions. VariableReferenceExpressionLine( VariableName = "p" ),
                              Parameters = [|new Expressions.VariableReferenceExpressionLine()|] ) )

        // TODO: %s

        let newInterface = new InterfaceBuilder("TestInterface")
        newInterface.AddVoidMethod("StartService",[||]).AddProperty<bool>("IsServiceStarted")
                            .AddMethod<string>("GetResult",[| new ParameterItem( Name = "studentName", Type = typeof<string>)|]) |> ignore


        let statementLine5 = new Statements.IterationStatementLine()
        statementLine5.Init <- null

        //statementLine5.Init = new Statement
        //statementLine5.Increment = new Statement
        //statementLine5.StatementLines = new Statement[]
        //foreach(var x in p.Run())\nyield return x;\n}
        (*
        IEnumerator e = dt.Rows.GetEnumerator();
        while(e.MoveNext())
        {
            DataRow row = (DataRow)e.Current;
            myList.Add(ConvertToObject(row));
        }*)
        
        let returnStatement2 = new Statements.ReturnStatementLine()
        returnStatement2.Expressions.Add( 0, new Expressions.VariableReferenceExpressionLine( VariableName = "result") )

        // Add the complete method to the class
        cb.AddMethod<bool>("Print", parameterItems, CodeDom.MemberAttributes.Public ||| CodeDom.MemberAttributes.Static, [|statementLine;returnStatement|] ) |> ignore
        cb.AddMethod<System.Object>("Run", parameterItems, CodeDom.MemberAttributes.Public ||| CodeDom.MemberAttributes.Static, [|statementLine2;statementLine3;statementLine4;returnStatement2|] ) |> ignore
        //cb.AddMethod<System.Collections.Generic.IEnumerable<System.Object>>("Printe", parameterItems, CodeDom.MemberAttributes.Public ||| CodeDom.MemberAttributes.Static, [|statementLine;returnStatement|] ) |> ignore

        let cub = new CompileUnitBuilder("Bla")
        let typeDecl = cb.GetDeclaration()
        cub.AddClass(typeDecl) |> ignore
        cub.AddInterface(newInterface.GetDeclaration()) |> ignore
        cub.PublishCode("c:\\Users\\Sytse\\Documents\\GitHubVisualStudio\\Minor_CodeGen\\CodeGen\\test.cs")
        
        //nameSpace.Classes.Add( cb )
        sprintf "public class EntryPoint {\n public static bool Print(string s) {System.Console.WriteLine(s); return true;}\n   \nstatic public object Run(bool printInput)\n{\n #line 1 \"input\"\n var p = %s;\nif(printInput) System.Console.WriteLine(p.ToString());\n %s\n var result = p.Run(); %s\n\nreturn result;\n}\n}\n" 
                (createElement ctxt programTyped |> fst) OptionalPrintHead OptionalPrintTail
      | KeywordMulteplicity.Multiple ->

        // Create the class
        let cb = new ClassBuilder("EntryPoint", TypeAttributes.Public, false)
        
        // Create the input parameter of the function
        let parameterItems = [|new ParameterItem( Name = "s", Type = typeof<string>)|]
        
        // Create the return statement
        let returnStatement = new Statements.ReturnStatementLine()
        returnStatement.Expressions.Add( 0, new Expressions.PrimitiveExpressionLine( Value = true ) )

        // Create the execute statements (like WriteLine)
        let statementLine = new Statements.ExpressionStatementLine()
        statementLine.Expressions.Add( 0, new Expressions.MethodInvokeExpressionLine
                            ( MethodName = "WriteLine", 
                              TargetObject = new Expressions.TypeReferenceExpressionLine( Type = "System.Console" ), 
                              Parameters = [|new Expressions.VariableReferenceExpressionLine( VariableName = "s" )|] ) )

        // Add the complete method to the class
        cb.AddMethod<bool>("Print", parameterItems, CodeDom.MemberAttributes.Public ||| CodeDom.MemberAttributes.Static, [|statementLine;returnStatement|] ) |> ignore
        cb.AddMethod<System.Collections.Generic.IEnumerable<System.Object>>("Print", parameterItems, CodeDom.MemberAttributes.Public ||| CodeDom.MemberAttributes.Static, [|statementLine;returnStatement|] ) |> ignore

        sprintf "public class EntryPoint {\n public static bool Print(string s) {System.Console.WriteLine(s); return true;}\n   static public IEnumerable<object> Run(bool printInput)\n{\n #line 1 \"input\"\n var p = %s;\nif(printInput) System.Console.WriteLine(p.ToString());\nforeach(var x in p.Run())\nyield return x;\n}\n}\n" 
                (createElement ctxt programTyped |> fst)
    [
      yield imports
      yield prelude
      yield "\n\n\n"
      yield interfacesCode
      yield "\n\n\n"
      for c in classes do
        let c = c.Value
        yield c.ToString(all_method_paths, ctxt, originalFilePath)
      yield "\n\n\n"
      yield main
      yield "\n}\n"
    ] |> Seq.fold (+) "" |> (fun txt -> System.IO.File.WriteAllText(program_name + ".cs", txt); program_name + ".cs")
  | _ -> failwithf "Cannot extract rules from input program @ %A" rules.DebugInformation
