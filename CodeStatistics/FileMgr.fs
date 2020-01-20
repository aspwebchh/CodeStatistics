module FileMgr

open System;
open System.IO
open System.Threading.Tasks
open Structs

let rec GetFileList path : Structs.PathItem List = 
    if File.Exists path then 
        [{Path=path;IsFile=true}]
    else 
        let dirs = Directory.GetDirectories( path )
        let files = Directory.GetFiles(path)
        let itemList = List.append (List.ofArray dirs) (List.ofArray files)
        let result = List.collect (fun it -> GetFileList it) itemList
        result

let IsCodeFile (extName : string) = 
    let codeExts = [".java";".kt";".php";".cs";".fs";".go";".js";".ts";".sql";".html";".htm";".css";".json";".xml";".txt";".text";".log";]
    List.exists (fun it -> String.Equals(it, extName.ToLower()) ) codeExts

let CetLineCount2 (path : string) = 
    use streamReader = new StreamReader(path)
    let content = streamReader.ReadToEnd()
    let items = content.Split('\n') |> Array.filter (fun it -> not(String.IsNullOrEmpty(it.Trim())) )
    items.Length
                   

let GetFileInfo path = 
    if not (File.Exists path) then 
        None
    else
        try 
            let extName = Path.GetExtension(path)
            let lineCount = if IsCodeFile(extName) then CetLineCount2( path ) else 0;
            use file = File.Open( path, FileMode.Open)
            let fileInfo : Structs.FileInfo = {
                Path=path;
                Name=file.Name;
                ExtName= extName;
                Size= Convert.ToInt32(file.Length) / 1000;
                LineCount= lineCount;
            }
            Some(fileInfo)
        with e ->  None


let getStatInfo  projectPath = 
    let fileInfo : Structs.PathItem List = GetFileList projectPath
    let filePaths = List.filter(fun  it -> it.IsFile) fileInfo 

    let fileInfoList = filePaths |> 
                        List.map (fun it -> Task.Run( fun() -> GetFileInfo it.Path)) |>
                        List.map (fun it -> it.Result ) |>
                        List.filter (fun it -> it.IsSome) |> List.map(fun it -> it.Value)
      
    let sumMapper (groupList :(string * FileInfo List)) : (string * int * int) =
        let (a,b) = groupList
        let totalFileCount = b.Length
        let totalLineCount = List.sumBy (fun (it:FileInfo) -> it.LineCount) b
        (a, totalFileCount, totalLineCount)
        
    let sumGroup = List.groupBy (fun it -> it.ExtName ) fileInfoList |> 
                    List.map sumMapper |> 
                    List.map (fun(a,b,c) -> {FileType=a;FileCount=b;TextLineCount=c}) |> 
                    List.sortByDescending ( fun it -> it.TextLineCount)
    
    {GroupInfo = sumGroup}

    