import { ClassSchema } from '../classSchema';

///////////////////////////////////////////
// RES_AllConfiguration.
export interface RES_AllConfiguration {
  schemaNames: Array<string>;
}

///////////////////////////////////////////
// REQ_CreateConfigSchema
export interface REQ_CreateConfigSchema {
  schema: ClassSchema;
}

// RES_CreateConfigSchema
export interface RES_CreateConfigSchema {}

///////////////////////////////////////////
// REQ_GetConfigSchemaInfo
export interface REQ_GetConfigSchemaInfo {
}

// RES_GetConfigSchemaInfo
export interface RES_GetConfigSchemaInfo {
  schema: ClassSchema;
}

