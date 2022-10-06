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
    url: 'api/centers',
    method: 'POST',
    contextType: 'application/json',
  },
  updateCenter: {
    url: 'api/centers/{0}',
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
export const CourseFamilyApis = {
  getAllCourseFamily: {
    url: 'api/course-families',
    method: 'GET',
    contextType: 'application/json',
  },
  updateCourseFamily: {
    url: 'api/course-families/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  createCourseFamily: {
    url: 'api/course-families',
    method: 'POST',
    contextType: 'application/json',
  },

};
export const CourseApis = {
  getAllCourse: {
    url: 'api/courses',
    method: 'GET',
    contextType: 'application/json',
  },
  updateCourse: {
    url: 'api/courses/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  createCourse: {
    url: '/api/courses',
    method: 'POST',
    contextType: 'application/json',
  },

};