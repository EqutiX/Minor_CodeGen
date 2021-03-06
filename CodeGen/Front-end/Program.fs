﻿open System
open Utilities
open ParserMonad
open BasicExpression
open ConcreteExpressionParserPrelude
open ConcreteExpressionParser
open CodeGenerator
open Microsoft.CSharp
open System.CodeDom.Compiler
open System.IO
open System.Runtime.Serialization
open System.Xml.Serialization
open AnalyserAST
open AssemblyPrecaching

do System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo.GetCultureInfo("EN-US")

let  readFiles (path : string) : List<string>= 
      let stringSeq = path.Split([|'\\'|])
      let stringList = stringSeq |> Seq.toList 
      let rec crawlFiles (s : List<string>) (currentDirect : string) =
        match s with
        | [] -> Directory.GetFiles(currentDirect)|> Seq.toList
        | x::xs -> 
          match Directory.Exists(x) with
          | true ->  crawlFiles xs (currentDirect+x)
          | false -> match File.Exists(x) with
                     | true -> [(x + currentDirect)]
                     | false -> failwith "The specified file and/or directory doesn't exist"
      crawlFiles (stringList) ("")
      

let runDeduction path =
        let originalFilePath = path
        let rules = System.IO.File.ReadAllText(originalFilePath)// + "\r\n"
        let title = System.IO.Path.GetFileName path
        let timer = System.Diagnostics.Stopwatch()
        let output = ref ""
        let addOutput s = output := sprintf "%s\n%s" (output.Value) s
        match (program()).Parse (rules |> Seq.toList) ConcreteExpressionContext.Empty (Position.Create originalFilePath) with
        | First(x,_,ctxt,pos) -> 
          fun (input:string) ->
            let input = input.Trim([|'\r'; '\n'|]) + "\n"
      //      do debug_expr <- true
            match expr().Parse (input |> Seq.toList) ctxt Position.Zero with
            | First(y,_,ctxt',pos') ->
              try
              let customDlls = ["UnityEngine.dll"; "System.Collections.Immutable.dll"]
              let defaultDlls = [ "mscorlib.dll"; "System.dll"; "System.Runtime.dll"; "System.Core.dll"] 
              let dllParam = Array.append (List.toArray defaultDlls) (List.toArray customDlls)
              let ctxt = { ctxt with AssemblyInfo = assemblyPrecache defaultDlls customDlls }
              if CompilerSwitches.useGraphBasedCodeGenerator then 
                GraphBasedCodeGenerator.generate originalFilePath title x y ctxt 
              let generatedPath = generateCode originalFilePath title x y ctxt
              let programToAnalyser = convert x
              //let customKeywordsToAnalyser = ctxt.CustomKeywords |> List.map(fun keyword -> convert keyword)
              let args = new System.Collections.Generic.Dictionary<string, string>()
              do args.Add("CompilerVersion", "v4.5")
              
              let csc = new CSharpCodeProvider()
              let parameters = new CompilerParameters(dllParam, sprintf "%s.dll" title, true)
              do parameters.GenerateInMemory <- true
              do parameters.CompilerOptions <- @"/optimize+"
              let results = csc.CompileAssemblyFromFile(parameters, [|generatedPath.ToString()|])
              if results.Errors.HasErrors then
                for error in results.Errors do
                  if error.IsWarning |> not then
                    do sprintf "%s at %d: %s" error.FileName error.Line error.ErrorText |> addOutput 
                if CompilerSwitches.flushCSFileOnError then
                  do System.IO.File.WriteAllText(generatedPath.ToString(), "")
              else
                let types = results.CompiledAssembly.GetTypes()
                let entryPoint = types |> Seq.find (fun t -> t.Name = "EntryPoint")
                let run = entryPoint.GetMethod("Run")
                let results =
                  match run.Invoke(null, [|false|]) with
                  | :? seq<obj> as res -> res |> Seq.toList
                  | res -> [res]
                do timer.Start()
                for i = 1 to CompilerSwitches.numProfilerRuns do
                  match run.Invoke(null, [|false|]) with
                  | :? seq<obj> as res -> res |> Seq.toList |> ignore
                  | res -> ()
                do timer.Stop()
                for r in results do sprintf "%A" r  |> addOutput 
                do "\n" |> addOutput 
                if CompilerSwitches.numProfilerRuns > 0 then
                  do sprintf "Total elapsed time per iteration = %gms" (float timer.ElapsedMilliseconds / float CompilerSwitches.numProfilerRuns) |> addOutput
                else
                  do sprintf "Total elapsed time per iteration = 0ms" |> addOutput
              with
              | e ->
                e.Message |> addOutput  
              output.Value
            | Second errors -> 
              sprintf "Parse error(s) in program at\n%s." ([errors] |> Error.Distinct |> Seq.map (fun e -> e.Line |> string) |> Seq.reduce (fun s x -> sprintf "%s\n%s" s x)) |> addOutput
              output.Value
        | Second errors ->
          fun (input:string) ->
            sprintf "Parse error(s) in rules at lines %s." ([errors] |> Error.Distinct |> Seq.map (fun e -> e.Line |> string) |> Seq.reduce (fun s x -> sprintf "%s\n%s" s x)) |> addOutput
            output.Value


[<EntryPoint; STAThread>]
let main argv = 
  let samples = 
    [
//      "CNV3/Statements.mc", "run"
//      "Sequence/seq.mc", "evals bb"
//      "CNV3/Tuples.mc", "fst (1.0,2.0)"
//      "Test/test.mc", "debug"
//      "CNV3/Basics.mc", "test"

//      "CodegenTest/ListTest.mc", "length 5::(4::(3::(2::(1::nil))))"

      "Boolean expressions/transform.mc", "run"

      "PeanoNumbers/transform.mc", "run"
      "Lists/transform.mc", "plus 0;1;2;3;nil 10"
      "Lists/transform.mc", "length 0;1;2;3;nil"
      "Lists/transform.mc", "contains 0;1;2;3;nil 2"
      "Lists/transform.mc", "removeOdd 0;1;2;3;nil"
      "Lists/transform.mc", "add 0;1;2;3;nil"

      "stsil/transform.mc", "dda lin snoc 3 snoc 2 snoc 1"
      
      "Eval without memory/transform.mc", "run"
      "Eval with readonly memory/transform.mc", "run (map <<ImmutableDictionary<string, int>.Empty>>)"
      "Eval with memory/transform.mc", "run (map <<ImmutableDictionary<string, int>.Empty>>)"
      "Eval with memory and control flow/transform.mc", "run (map <<ImmutableDictionary<string, Value>.Empty>>)"

      "Binary trees/transform.mc", "run"

      "Trees 234/transform.mc", @"main"

      "Lambda calculus/transform.mc", "run"

      "GenericLists/transform.mc", @"length 0::1::2::3::nil"
      "GenericLists/transform.mc", @"length ""0""::""1""::""2""::""3""::nil"
      
// not yet converted to new keyword syntax:


      //"Cmm", @"runProgram"
//      "Casanova semantics", @"runTest1"
    ]

  //for name,input in samples 
  //  do runDeduction (System.IO.Path.Combine([| @"..\..\..\Content\Content"; name|])) input |> printfn "%s"

  do GUI.ShowGUI samples runDeduction

  0
