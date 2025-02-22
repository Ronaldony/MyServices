import { RES_Error } from './../classes/shared/protocols/error.ts';
import axios, { HttpStatusCode } from 'axios';

// Request Get method.
export async function Request_Get<T>(path): Promise<T | RES_Error> {
  try {
    const apiBase = window.sessionStorage.getItem(process.env.REACT_APP_API_BASE_SESSION ?? "");
    const res = await axios.get(`${apiBase}${path}`);
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
export async function Request_Post<T>(path, body): Promise<T | RES_Error> {
  try {
    const apiBase = window.sessionStorage.getItem(process.env.REACT_APP_API_BASE_SESSION ?? "");
    const res = await axios.post(`${apiBase}${path}`, body);
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
export async function Request_Patch<T>(path, body): Promise<T | RES_Error> {
  try {
    const apiBase = window.sessionStorage.getItem(process.env.REACT_APP_API_BASE_SESSION ?? "");
    const res = await axios.patch(`${apiBase}${path}`, body);
    const resData = res.data;

    // check status code.
    if (resData?.errorCode)
      return new RES_Error(resData.errorCode, resData.errorMsg);

    return resData;
  } catch (error) {
    return new RES_Error(HttpStatusCode.NotFound, 'API request failed');
  }
}

// Request Delete method.
export async function Request_Delete<T>(path, body): Promise<T | RES_Error> {
  try {
    const apiBase = window.sessionStorage.getItem(process.env.REACT_APP_API_BASE_SESSION ?? "");
    const res = await axios.delete(`${apiBase}${path}`, { data: body });
    const resData = res.data;

    // check status code.
    if (resData?.errorCode)
      return new RES_Error(resData.errorCode, resData.errorMsg);

    return resData;
  } catch (error) {
    return new RES_Error(HttpStatusCode.NotFound, 'API request failed');
  }
}
