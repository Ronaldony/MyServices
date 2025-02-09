///////////////////////////////////////////
// ClassSchemaColumn
export class ClassSchemaColumn {
  idx: number;
  name: string;
  isIndexed: boolean;
  type: string;
  refTable: string;
  summary: string;
  
  // constructor
  constructor(idx: number, name: string, isIndexed: boolean, type: string, refTable: string, summary: string) {
    this.idx = idx;
    this.name = name;
    this.isIndexed = isIndexed;
    this.type = type;
    this.refTable = refTable;
    this.summary = summary;
  }
}

// ClassSchema.
export class ClassSchema {
  name: string;
  category: string;
  description: string;
  columns: Array<ClassSchemaColumn> | null;
  
  // constructor
  constructor(name: string, category: string, description: string, columns: Array<ClassSchemaColumn> | null) {
    this.name = name;
    this.category = category;
    this.description = description;
    this.columns = columns;
  }
}

// ClassData
export class ClassData {
    schema: ClassSchema;
    rows: Array<any>;
        
    // constructor 
    constructor(schema: ClassSchema, rows: Array<any>)
    {
        this.schema = schema;
        this.rows = rows;
    }
}