// Profile
export class Profile
{
    name: string;
    filePathList: Array<Profile_FilePath>;
    
    // constructor
    constructor(name: string, filePathList: Array<Profile_FilePath>)
    {
        this.name = name;
        this.filePathList = filePathList;
    }
}

// Profile_FilePath
export class Profile_FilePath
{
    target: string;
    protocolPath: string;
    dataCodePath: string;
    databaseFilePath: string;
}