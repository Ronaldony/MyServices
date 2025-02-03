import { RES_Error } from './../classes/shared/protocols/error.ts';
import axios, { HttpStatusCode } from 'axios';

// Request Get method.
export async function Request_Get<T>(url): Promise<T | RES_Error> {
  try {
    const res = await axios.get(url);
    const resData = res.data;

    // check status code.
    if (resData?.errorCode)
      return new RES_Error(resData.errorCode, resData.errorMsg);

    return resData;
  } catch (error) {
    return new RES_Error(HttpStatusCode.NotFound, 'API request failed');
  }
}

// Request Post method.
export async function Request_Post<T>(url, body): Promise<T | RES_Error> {
  try {
    const res = await axios.post(url, body);
    const resData = res.data;

    // check status code.
    if (resData?.errorCode)
      return new RES_Error(resData.errorCode, resData.errorMsg);

    return resData;
  } catch (error) {
    return new RES_Error(HttpStatusCode.NotFound, 'API request failed');
  }
}

// Request Patch method.
export function Request_Patch(url, body) {}

// Request Delete method.
export function Request_Delete(url) {}
