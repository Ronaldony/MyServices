///////////////////////////////////////////
///////////////////////////////////////////
// REQ_FetchTypeAndTableKeys.
export class REQ_FetchTypeAndTableKeys {

}

// RES_FetchTypeAndTableKeys.
export class RES_FetchTypeAndTableKeys {
    schemaKeysDict: Map<string, []>;
}

///////////////////////////////////////////
///////////////////////////////////////////
// REQ_FetchRefTables.
export class REQ_FetchRefTables
{
}

// RES_FetchRefTables.
export class RES_FetchRefTables
{
    tables: Array<string>
}

///////////////////////////////////////////
///////////////////////////////////////////
// REQ_FetchTypes.
export class REQ_FetchTypes
{

}

// RES_FetchTypes.
export class RES_FetchTypes
{
    types: Array<string>
}