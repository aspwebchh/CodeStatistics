module Structs

type PathItem = {Path:string; IsFile:bool}


type FileInfo = {
    Path:string;
    Name:string;
    ExtName:string;
    Size:int;
    LineCount:int;
}

type GroupInfoItem = {
    FileType:string;
    FileCount:int;
    TextLineCount:int
}

type Result = {
    GroupInfo: GroupInfoItem List;
}   