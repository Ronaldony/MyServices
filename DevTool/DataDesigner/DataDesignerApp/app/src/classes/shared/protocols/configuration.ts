import { ClassSchema } from '../classSchema';

///////////////////////////////////////////
// RES_AllConfiguration.
export class RES_AllConfiguration {
  schemaNames: Array<string>;

  // constructor
  constructor(schemaNames: Array<string>) {
    this.schemaNames = schemaNames;
  }
}

///////////////////////////////////////////
// REQ_CreateConfigSchema
export class REQ_CreateConfigSchema {
  schema: ClassSchema;

  // constructor
  constructor(schema: ClassSchema) {
    this.schema = schema;
  }
}

// RES_CreateConfigSchema
export class RES_CreateConfigSchema {}

///////////////////////////////////////////
// REQ_FindConfigSchema
export class REQ_FindConfigSchema {}

// RES_GetConfigSchemaInfo
export class RES_FindConfigSchema {
  schema: ClassSchema;

  // constructor
  constructor(schema: ClassSchema) {
    this.schema = schema;
  }
}

///////////////////////////////////////////
// REQ_FindConfigRows.
export class REQ_FindConfigRows {}

// RES_FindConfigRows.
export class RES_FindConfigRows {
  schema: string;
  rows: Array<any>;

  constructor(schema: string, rows: Array<any>) {
    this.schema = schema;
    this.rows = rows;
  }
}
