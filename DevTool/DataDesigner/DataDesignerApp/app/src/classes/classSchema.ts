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
  desciption: string;
  columns: Array<ClassSchemaColumn> | null;
  
  // constructor
  constructor(name: string, category: string, desciption: string, columns: Array<ClassSchemaColumn> | null) {
    this.name = name;
    this.category = category;
    this.desciption = desciption;
    this.columns = columns;
  }
}
