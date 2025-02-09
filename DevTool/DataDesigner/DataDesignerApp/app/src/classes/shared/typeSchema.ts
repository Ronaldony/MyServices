
///////////////////////////////////////////
// TypeSchema
export class TypeSchema {
    name: string;
    category: string;
    description: string;

    // constructor 
    constructor(name: string, category: string, description: string)
    {
        this.name = name;
        this.category = category;
        this.description = description;
    }
}

// TypeSchemaRow
export class TypeSchemaRow
{
    id: number;
    name: string;
    summary: string;

    // constructor 
    constructor(id: number, name: string, summary: string)
    {
        this.id = id;
        this.name = name;
        this.summary = summary;
    }
}

// TypeSchemaColumn.
export class TypeSchemaColumn 
{
    name: string;
    type: string;

    // constructor 
    constructor(name: string, type: string)
    {
        this.name = name;
        this.type = type;
    }
}

// TypeData
export class TypeData {
    schema: TypeSchema;
    rows: Array<any>;
        
    // constructor 
    constructor(schema: TypeSchema, rows: Array<any>)
    {
        this.schema = schema;
        this.rows = rows;
    }
}