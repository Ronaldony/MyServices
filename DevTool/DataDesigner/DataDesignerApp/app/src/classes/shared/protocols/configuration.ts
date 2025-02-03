import { ClassSchema } from '../shared/classSchema';

///////////////////////////////////////////
// RES_AllConfiguration.
export class RES_AllConfiguration {
  schemaNames: Array<string>;

  // constructor
  constructor(schemaNames:  Array<string>) {
    this.schemaNames = schemaNames;
  }
}

///////////////////////////////////////////
// REQ_CreateConfigSchema
export class REQ_CreateConfigSchema {
  schema: ClassSchema;

  // constructor
  constructor(schema:  ClassSchema) {
    this.schema = schema;
  }
}

// RES_CreateConfigSchema
export class RES_CreateConfigSchema {}

///////////////////////////////////////////
// REQ_FindConfigSchema
export class REQ_FindConfigSchema {
}

// RES_GetConfigSchemaInfo
export class RES_FindConfigSchema {
  schema: ClassSchema;
  
  // constructor
  constructor(schema:  ClassSchema) {
    this.schema = schema;
  }
}