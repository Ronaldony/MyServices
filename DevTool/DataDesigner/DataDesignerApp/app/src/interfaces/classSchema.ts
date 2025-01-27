// ClassSchemaColumn
export interface ClassSchemaColumn {
  idx: number;
  name: string;
  isIndexed: boolean;
  type: string;
  refType: any;
  summary: string;
}

// ClassSchema.
export interface ClassSchema {
  name: string,
  category: string
  desciption: string,
  columns: Array<ClassSchemaColumn>
}