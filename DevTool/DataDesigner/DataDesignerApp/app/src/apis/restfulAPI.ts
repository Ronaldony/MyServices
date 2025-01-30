import axios from 'axios';

// Request Get method.
export async function Request_Get<T>(url): Promise<T | null> {
  try {
    const res = await axios.get(url);
    const resData = res.data;

    // check status code.
    if (resData?.errorCode) {
      console.log(
        `server error code: ${resData.errorCode}, msg: ${resData.errorMsg}`
      );
      return null;
    }

    return resData;
  } catch (error) {
    console.log(`api error - ${error}`);
    return null;
  }
}

// Request Post method.
export async function Request_Post<T>(url, body): Promise<T | null> {
  try {
    const res = await axios.post(url, body);
    const resData = res.data;

    // check status code.
    if (resData?.errorCode) {
      console.log(
        `server error code: ${resData.errorCode}, msg: ${resData.errorMsg}`
      );
      return null;
    }

    return resData;
  } catch (error) {
    console.log(`api error - ${error}`);
    return null;
  }
}

// Request Patch method.
export function Request_Patch(url, body) {}

// Request Delete method.
export function Request_Delete(url) {}