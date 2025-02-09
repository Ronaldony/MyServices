///////////////////////////////////////////

import { TypeSchema, TypeSchemaColumn } from '../typeSchema';

// REQ_GetAllType
export class REQ_GetAllType {}

// RES_GetAllType
export class RES_GetAllType {
  schemaNames: Array<string>;
}

///////////////////////////////////////////
// REQ_CreateTypeSchema.
export class REQ_CreateTypeSchema {
  schema: TypeSchema;

  constructor(schema: TypeSchema) {
    this.schema = schema;
  }
}

// RES_CreateTypeSchema.
export class RES_CreateTypeSchema {}

///////////////////////////////////////////
// REQ_FindTypeRows.
export class REQ_FindTypeRows {}

// RES_FindTypeRows
export class RES_FindTypeRows {
  schema: TypeSchema;
  rows: Array<any>;

  // constructor
  constructor(schema: TypeSchema, rows: Array<any>) {
    this.schema = schema;
    this.rows = rows;
  }
}

///////////////////////////////////////////
// REQ_FindTypeColumns.
export class REQ_FindTypeColumns {}

// RES_FindTypeColumns
export class RES_FindTypeColumns {
  columns: Array<TypeSchemaColumn>;

  // constructor
  constructor(columns: Array<TypeSchemaColumn>) {
    this.columns = columns;
  }
}

///////////////////////////////////////////
// REQ_FindTypeSchema.
export class REQ_FindTypeSchema {}

// RES_FindTypeSchema
export class RES_FindTypeSchema {
  schema: TypeSchema;

  // constructor
  constructor(schema: TypeSchema) {
    this.schema = schema;
  }
}

///////////////////////////////////////////
// REQ_DeleteTypeSchema.
export class REQ_DeleteTypeSchema {
  schema: string;

  // constructor
  constructor(schema: string) {
    this.schema = schema;
  }
}

// RES_DeleteTypeSchema
export class RES_DeleteTypeSchema {}

///////////////////////////////////////////
// REQ_UpdateTypeSchema.
export class REQ_UpdateTypeSchema {
  schema: TypeSchema;

  // constructor
  constructor(schema: TypeSchema) {
    this.schema = schema;
  }
}

// RES_UpdateTypeSchema
export class RES_UpdateTypeSchema {}
