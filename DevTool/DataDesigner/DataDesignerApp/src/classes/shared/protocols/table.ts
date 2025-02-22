import { ClassSchema } from '../classSchema';

///////////////////////////////////////////
// RES_GetAllTable.
export class RES_GetAllTable {
  schemaNames: Array<string>;

  // constructor
  constructor(schemaNames: Array<string>) {
    this.schemaNames = schemaNames;
  }
}

///////////////////////////////////////////
// REQ_CreateTableSchema
export class REQ_CreateTableSchema {
  schema: ClassSchema;

  // constructor
  constructor(schema: ClassSchema) {
    this.schema = schema;
  }
}

// RES_CreateTableSchema
export class RES_CreateTableSchema {}

///////////////////////////////////////////
// REQ_UpdateTableSchema
export class REQ_UpdateTableSchema {
  schema: ClassSchema;

  // constructor
  constructor(schema: ClassSchema) {
    this.schema = schema;
  }
}

// RES_UpdateTableSchema
export class RES_UpdateTableSchema {}

///////////////////////////////////////////
// REQ_DeleteTableSchema
export class REQ_DeleteTableSchema
{
  schema: string;

  // constructor
  constructor(schema: string) {
    this.schema = schema;
  }
}

// RES_DeleteTableSchema
export class RES_DeleteTableSchema
{
}

///////////////////////////////////////////
// REQ_FindTableSchema
export class REQ_FindTableSchema {}

// RES_GetTableSchemaInfo
export class RES_FindTableSchema {
  schema: ClassSchema;

  // constructor
  constructor(schema: ClassSchema) {
    this.schema = schema;
  }
}

///////////////////////////////////////////
// REQ_FindCTableRows.
export class REQ_FindTableRows {}

// RES_FindTableRows.
export class RES_FindTableRows {
  schema: ClassSchema;
  rows: Array<any>;

  constructor(schema: ClassSchema, rows: Array<any>) {
    this.schema = schema;
    this.rows = rows;
  }
}

///////////////////////////////////////////
// REQ_UpdateTableRows.
export class REQ_UpdateTableRows {
  schema: string;
  rows: Array<any>;

  constructor(schema: string, rows: Array<any>) {
    this.schema = schema;
    this.rows = rows;
  }
}

// RES_UpdateTableRows.
export class RES_UpdateTableRows {
}
