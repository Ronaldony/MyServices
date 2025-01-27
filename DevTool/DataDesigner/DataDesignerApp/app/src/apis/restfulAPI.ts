import axios from 'axios';

// Request Get method.
export async function Request_Get<T>(url): Promise<T | null> 
{
  try {
    const res = await axios.get(url);
    const recvData = res.data;

    return recvData;
  } catch (error) {
    console.log(error);
    return null;
  }
}

// Request Post method.
export async function Request_Post<T>(url, body): Promise<T | null>
{
  try {
    const res = await axios.post(url, body);
    const recvData = res.data;

    return recvData;
  } catch (error) {
    console.log(error);
    return null;
  }
}

// Request Patch method.
export function Request_Patch(url, body) {}

// Request Delete method.
export function Request_Delete(url) {}
