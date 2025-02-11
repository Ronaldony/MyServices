import { Profile } from '../profile';

/////////////////////////////////////
// REQ_DeleteProfile
export class REQ_DeleteProfile {
  name: string;

  // constructor
  constructor(name: string) {
    this.name = name;
  }
}

// RES_DeleteProfile
export class RES_DeleteProfile {}

/////////////////////////////////////
// REQ_FindProfile
export class REQ_FindProfile {}

// RES_FindProfile.
export class RES_FindProfile {
  profile: Profile;
}

/////////////////////////////////////
// REQ_GetProfileNames
export class REQ_GetProfileNames {}

// RES_GetProfileNames.
export class RES_GetProfileNames {
  profileNames: Array<string>;
}

// REQ_UpdateProfile
export class REQ_UpdateProfile {
  profile: Profile;

  constructor(profile: Profile) {
    this.profile = profile;
  }
}

// RES_UpdateProfile.
export class RES_UpdateProfile {}

//////////////////////////////////
// REQ_CreateProfile
export class REQ_CreateProfile {
  name: string;

  constructor(name: string){
    this.name = name;
  }
}

// RES_CreateProfile.
export class RES_CreateProfile {}

// REQ_Generate
export class REQ_Generate{
  target: string;
  profile: Profile;

  constructor(target: string, profile: Profile)
  {
    this.target = target;
    this.profile = profile;
  }
}

// RES_Generate
export class RES_Generate{}