
///////////////////////////////////////////
// RES_Error
export class RES_Error {
  errorCode: number;
  errorMsg: string;

  // constructor
  constructor(errorCode: number, errorMsg: string) {
    this.errorMsg = errorMsg;
    this.errorCode = errorCode;
  }
}
