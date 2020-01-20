// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
module Test

open System
open System.Threading.Tasks
open FileMgr
open Structs

[<EntryPoint>]
let main argv =
    Console.Write("请输入项目路径：");
    let projectPath = Console.ReadLine()

    let result = FileMgr.getStatInfo projectPath
    let sumGroup = result.GroupInfo

    let repeatSpace count = 
        let array = [|for i in 0..count -> " "|]
        String.Join("", array)

    let formatItem value : string = 
        value + repeatSpace (15 - value.Length)

    let resultStrItems = List.map ( fun it -> String.Format("文件类型：{0} 文件数：{1} 文本行数：{2}", formatItem it.FileType, formatItem(it.FileCount.ToString()), formatItem(it.TextLineCount.ToString()) )) sumGroup |>  List.toArray
    let resultString = String.Join("\n", resultStrItems)
    Console.WriteLine("")
    Console.WriteLine(resultString)
    Console.Write("\n按任意键退出")
    Console.ReadKey() |> ignore
    0

