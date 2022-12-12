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
  getCenterById: {
    url: 'api/centers/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  checkCanDeleteCenter: {
    url: 'api/centers/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  changeStatusCenter: {
    url: 'api/centers/{0}/change-activate',
    method: 'PATCH',
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
  deleteCenter: {
    url: 'api/centers/{0}',
    method: 'DELETE',
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

export const SemesterApis = {
  getAllSemester: {
    url: 'api/semesters',
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
  ChangeStatus: {
    url: 'api/course-families/{0}',
    method: 'PATCH',
    contextType: 'application/json',
  },
  checkCanDeleteCourseFamily: {
    url: 'api/course-families/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  deleteCourseFamily: {
    url: 'api/course-families/{0}',
    method: 'DELETE',
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
    url: 'api/courses',
    method: 'POST',
    contextType: 'application/json',
  },
  getCourseByCode: {
    url: 'api/courses/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  checkCanDeleteCourse: {
    url: 'api/courses/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  deleteCourse: {
    url: 'api/courses/{0}',
    method: 'DELETE',
    contextType: 'application/json',
  },
};

export const ModulesApis = {
  getAllModules: {
    url: 'api/modules',
    method: 'GET',
    contextType: 'application/json',
  },
  searchModules: {
    url: 'api/modules/search',
    method: 'GET',
    contextType: 'application/json',
  },
  createModules: {
    url: 'api/modules',
    method: 'POST',
    contextType: 'application/json',
  },
  getModuleByID: {
    url: 'api/modules/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  updateModule: {
    url: 'api/modules/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  checkCanDeleteModule: {
    url: 'api/modules/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  deleteCourse: {
    url: 'api/modules/{0}',
    method: 'DELETE',
    contextType: 'application/json',
  },
};

export const CourseModuleSemesterApis = {
  getByCourseCode: {
    url: 'api/courses-modules-semesters/courses/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  getAllCourseModuleSemesterApis: {
    url: 'api/courses-modules-semesters',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const RoomApis = {
  getAllRoom: {
    url: 'api/rooms',
    method: 'GET',
    contextType: 'application/json',
  },
  createRoom: {
    url: 'api/rooms',
    method: 'POST',
    contextType: 'application/json',
  },
  updateRoom: {
    url: 'api/rooms/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  checkCanDeleteRoom: {
    url: 'api/rooms/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  changeActiveRoom: {
    url: 'api/rooms/{0}/change-status',
    method: 'PATCH',
    contextType: 'application/json',
  },
  deleteRoom: {
    url: 'api/rooms/{0}',
    method: 'DELETE',
    contextType: 'application/json',
  },
  getRoomsBySro: {
    url: 'api/rooms/get-by-sro',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const RoomTypeApis = {
  getAllRoomType: {
    url: 'api/room-types',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const ManageSroApis = {
  getAllSro: {
    url: 'api/sros',
    method: 'GET',
    contextType: 'application/json',
  },
  searchSro: {
    url: 'api/sros/search',
    method: 'GET',
    contextType: 'application/json',
  },
  getDetailSro: {
    url: 'api/sros/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  createSro: {
    url: 'api/sros',
    method: 'POST',
    contextType: 'application/json',
  },
  updateSro: {
    url: 'api/sros/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  changeActive: {
    url: 'api/sros/{0}/change-active',
    method: 'PATCH',
    contextType: 'application/json',
  },
  checkCanDeleteSro: {
    url: 'api/sros/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  deleteSro: {
    url: 'api/sros/{0}',
    method: 'DELETE',
    contextType: 'application/json',
  },
};

export const GenderApis = {
  getAllGender: {
    url: 'api/genders',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const ManageTeacherApis = {
  searchTeacher: {
    url: 'api/teachers/search',
    method: 'GET',
    contextType: 'application/json',
  },
  searchTeacherBySro: {
    url: 'api/teachers/search-by-sro',
    method: 'GET',
    contextType: 'application/json',
  },
  detailTeacher: {
    url: 'api/teachers/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  getWorkingTime: {
    url: 'api/working-times',
    method: 'GET',
    contextType: 'application/json',
  },
  getTeacherType: {
    url: 'api/teacher-types',
    method: 'GET',
    contextType: 'application/json',
  },
  createTeacher: {
    url: 'api/teachers',
    method: 'POST',
    contextType: 'application/json',
  },
  updateTeacher: {
    url: 'api/teachers/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  getInformationTeacher: {
    url: 'api/teachers/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  changeActive: {
    url: 'api/teachers/{0}/change-active',
    method: 'PATCH',
    contextType: 'application/json',
  },
  getListSkillOfTeacher: {
    url: 'api/teachers/{0}/skills',
    method: 'GET',
    contextType: 'application/json',
  },
  saveSkillOfTeacher: {
    url: 'api/teachers/{0}/skills',
    method: 'POST',
    contextType: 'application/json',
  },
  checkCanDeleteTeacher: {
    url: 'api/teachers/{0}/can-delete',
    method: 'GET',
    contextType: 'application/json',
  },
  deleteTeacher: {
    url: 'api/teachers/{0}',
    method: 'DELETE',
    contextType: 'application/json',
  },
  getListTeacherBySro: {
    url: 'api/teachers/get-by-sro',
    method: 'GET',
    contextType: 'application/json',
  },
  getListModulesOfATeacher: {
    url: 'api/gpa/teachers/{0}/modules',
    method: 'GET',
    contextType: 'application/json',
  },
  getListClassOfATeacherByModule: {
    url: 'api/gpa/modules/{0}/classes',
    method: 'GET',
    contextType: 'application/json',
  },
  getAverageGPAOfATeacher: {
    url: 'api/gpa/teachers/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  getAverageGPAOfATeacherByModule: {
    url: 'api/gpa/teachers/{0}/modules/{1}',
    method: 'GET',
    contextType: 'application/json',
  },
  getAverageGPAOfATeacherByModuleAndClass: {
    url: 'api/gpa/teachers/{0}/classes/{1}/modules/{2}',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const UserApis = {
  getInformationUser: {
    url: 'api/users/information',
    method: 'GET',
    contextType: 'application/json',
  },
  changeAvatarForAdmin: {
    url: 'api/admin/users/{0}/avatar',
    method: 'POST',
    contextType: 'application/json',
  },
  changeAvatarForSro: {
    url: 'api/sro/users/{0}/avatar',
    method: 'POST',
    contextType: 'application/json',
  },
};

export const GradeType = {
  getAllGradeType: {
    url: 'api/grade-categories',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const GradeModuleSemesterApis = {
  getListGradeByModuleId: {
    url: 'api/modules/{0}/grades',
    method: 'GET',
    contextType: 'application/json',
  },
  updateGradeModule: {
    url: 'api/modules/{0}/grades',
    method: 'POST',
    contextType: 'application/json',
  },
};

export const GradeStudentApis = {
  getListGradeByClassIdAndModuleIdBySro: {
    url: 'api/classes/{0}/modules/{1}/grades-students/sros',
    method: 'GET',
    contextType: 'application/json',
  },
  getListGradeByClassIdAndModuleIdByTeacher: {
    url: 'api/classes/{0}/modules/{1}/grades-students/teachers',
    method: 'GET',
    contextType: 'application/json',
  },
  updateGradeStudentBySro: {
    url: 'api/classes/{0}/modules/{1}/grades/sros',
    method: 'POST',
    contextType: 'application/json',
  },
  updateGradeStudentByTeacher: {
    url: 'api/classes/{0}/modules/{1}/grades/teachers',
    method: 'POST',
    contextType: 'application/json',
  },
};

export const ManageClassApis = {
  searchClass: {
    url: 'api/classes/search',
    method: 'GET',
    contextType: 'application/json',
  },
  getAllClass: {
    url: 'api/classes',
    method: 'GET',
    contextType: 'application/json',
  },
  createClass: {
    url: 'api/classes',
    method: 'POST',
    contextType: 'application/json',
  },
  getInformationClass: {
    url: 'api/classes/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  updateClass: {
    url: 'api/classes/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  downloadTemplate: {
    url: 'api/classes/download-template-import-students',
    method: 'GET',
    contextType: 'application/json',
    responseType: 'blob',
  },
  importStudent: {
    url: 'api/classes/{0}/students-from-excel',
    method: 'POST',
    contextType: 'multipart/form-data',
  },
  getStudentByClassId: {
    url: 'api/classes/{0}/students',
    method: 'GET',
    contextType: 'application/json',
  },
  clearStudent: {
    url: 'api/classes/{0}/students-draft',
    method: 'DELETE',
    contextType: 'application/json',
  },
  saveStudent: {
    url: 'api/classes/{0}/students',
    method: 'PATCH',
    contextType: 'application/json',
  },
  addStudentToClass: {
    url: 'api/classes/{0}/students',
    method: 'POST',
    contextType: 'application/json',
  },
  getAllModulesOfClass: {
    url: 'api/classes/{0}/modules',
    method: 'GET',
    contextType: 'application/json',
  },
  mergeClass: {
    url: 'api/classes/merge',
    method: 'PUT',
    contextType: 'application/json',
  },
  getAvailableClassToMerge: {
    url: 'api/classes/{0}/available-to-merge',
    method: 'GET',
    contextType: 'application/json',
  },
  exportStudent: {
    url: 'api/classes/{0}/export-students',
    method: 'GET',
    contextType: 'application/json',
    responseType: 'blob',
  }
};

export const ManageScheduleApis = {
  getScheduleByClassIdAndModuleId: {
    url: 'api/classes/{0}/schedules/modules/{1}',
    method: 'GET',
    contextType: 'application/json',
  },
  createSchedule: {
    url: 'api/classes/{0}/schedules',
    method: 'POST',
    contextType: 'application/json',
  },
  updateSchedule: {
    url: 'api/classes-schedules/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  deleteSchedule: {
    url: 'api/classes/{0}/schedules/{1}',
    method: 'DELETE',
    contextType: 'application/json',
  },
  progress: {
    url: 'api/classes/progress',
    method: 'GET',
    contextType: 'application/json',
  },
  getDuliicateScheduleRoom: {
    url: 'api/classes-schedules/sessions-duplicate-room',
    method: 'GET',
    contextType: 'application/json',
  },
  getDuliicateScheduleTeacher: {
    url: 'api/classes-schedules/sessions-duplicate-teacher',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const ManageStudentApis = {
  searchStudent: {
    url: 'api/students/search',
    method: 'GET',
    contextType: 'application/json',
  },
  detailStudent: {
    url: 'api/students/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  getInformationStudent: {
    url: 'api/students/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  updateStudent: {
    url: 'api/students/{0}',
    method: 'PUT',
    contextType: 'application/json',
  },
  listAvailableClassToChange: {
    url: 'api/students/{0}/classes/available-to-change',
    method: 'GET',
    contextType: 'application/json',
  },
  changeClass: {
    url: 'api/students/{0}/change-class',
    method: 'PUT',
    contextType: 'application/json',
  },
  getCurrentClasss: {
    url: 'api/students/{0}/classes/',
    method: 'GET',
    contextType: 'application/json',
  },
  getStudentSemesterWithClassAndModuleBySro: {
    url: 'api/students/{0}/semesters/modules',
    method: 'GET',
    contextType: 'application/json',
  },
  getGradeStudentBySro: {
    url: 'api/classes/{0}/modules/{1}/students/{2}/grades',
    method: 'GET',
    contextType: 'application/json',
  },
  ChangeActive: {
    url: 'api/students/{0}/change-active',
    method: 'PATCH',
    contextType: 'application/json',
  },
};

export const ManageDayOffApis = {
  getDayOff: {
    url: 'api/days-off',
    method: 'GET',
    contextType: 'application/json',
  },
  getDetail: {
    url: 'api/days-off/detail',
    method: 'POST',
    contextType: 'application/json',
  },
  createDayOff: {
    url: 'api/days-off',
    method: 'POST',
    contextType: 'application/json',
  },
  deleteDayOff: {
    url: 'api/days-off/{0}',
    method: 'DELETE',
    contextType: 'application/json',
  },
};
export const ManageGpa = {
  getForm: {
    url: 'api/gpa/forms',
    method: 'GET',
    contextType: 'application/json',
  },
  getQuestionByFormID: {
    url: 'api/gpa/forms/{0}/questions',
    method: 'GET',
    contextType: 'application/json',
  },
  getAnswerByQuestionID: {
    url: 'api/gpa/forms/{0}/questions/{1}/answers',
    method: 'GET',
    contextType: 'application/json',
  },
  getQandA: {
    url: 'api/gpa/forms/{0}/questions/answers',
    method: 'GET',
    contextType: 'application/json',
  },
  studentTakeGPA: {
    url: 'api/gpa/forms/{0}',
    method: 'POST',
    contextType: 'application/json',
  },
  getScheduleInformationBySessionId: {
    url: 'api/gpa/sessions/{0}',
    method: 'GET',
    contextType: 'application/json',
  },
  requestStudentTakeGPA: {
    url: 'api/gpa/sessions/{0}/request-email',
    method: 'POST',
    contextType: 'application/json',
  },
};

export const ManageAttendanceApis = {
  getAttendanceByScheduleId: {
    url: 'api/classes-schedules/{0}/attendances',
    method: 'GET',
    contextType: 'application/json',
  },
  getAttendanceBySlotId: {
    url: 'api/classes-schedules/{0}/sessions/{1}/attendances',
    method: 'GET',
    contextType: 'application/json',
  },
  submitAttendanceSro: {
    url: 'api/classes-schedules/{0}/sessions/{1}/attendances/sros',
    method: 'POST',
    contextType: 'application/json',
  },
  submitAttendanceTeacher: {
    url: 'api/classes-schedules/{0}/sessions/{1}/attendances/teachers',
    method: 'POST',
    contextType: 'application/json',
  },
  getTeachingClass: {
    url: 'api/teaching-classes/teachers',
    method: 'GET',
    contextType: 'application/json',
  },
  teacherGetListSessions: {
    url: 'api/classes-schedules/{0}/sessions',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const UserStudentApis = {
  getScheduleStudent: {
    url: 'api/schedules/students',
    method: 'GET',
    contextType: 'application/json',
  },
  getAllSession: {
    url: 'api/sessions/students',
    method: 'GET',
    contextType: 'application/json',
  },
  getDetailSession: {
    url: 'api/sessions/detail/students/get',
    method: 'POST',
    contextType: 'application/json',
  },
  getModuleSemester: {
    url: 'api/students/semesters/modules',
    method: 'GET',
    contextType: 'application/json',
  },
  getGradesbyclass: {
    url: 'api/classes/{0}/modules/{1}/grades-students/students',
    method: 'GET',
    contextType: 'application/json',
  },
  getAttendance: {
    url: 'api/classes/{0}/modules/{1}/attendances/student',
    method: 'GET',
    contextType: 'application/json',
  },
};

export const UserTeacherApis = {
  getAllTeachSession: {
    url: 'api/sessions/teachers',
    method: 'GET',
    contextType: 'application/json',
  },
  getDetailTeachSession: {
    url: 'api/sessions/detail/teachers-get',
    method: 'POST',
    contextType: 'application/json',
  },
};
