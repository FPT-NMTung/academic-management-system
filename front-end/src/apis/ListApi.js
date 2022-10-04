export const AuthApis = {
  login: {
    url: 'api/auth/login',
    method: 'POST',
    contextType: 'application/json',
  },
};

export const CenterApis = {
  getAllCenter: {
    url: 'api/centers',
    method: 'GET',
    contextType: 'application/json',
  },
  createCenter: {
    url: 'api/center',
    method: 'POST',
    contextType: 'application/json',
  },
  updateCenter: {
    url: 'api/center',
    method: 'PUT',
    contextType: 'application/json',
  },
};

export const AddressApis = {
  getListProvince: {
    url: 'api/address/provinces',
    method: 'GET',
    contextType: 'application/json',
  },
  getListDistrict: {
    url: 'api/address/provinces/{0}/districts',
    method: 'GET',
    contextType: 'application/json',
  },
  getListWard: {
    url: 'api/address/provinces/{0}/districts/{1}/wards',
    method: 'GET',
    contextType: 'application/json',
  },
};