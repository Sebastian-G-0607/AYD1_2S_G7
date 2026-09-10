export { default as StudentProfileView } from './components/StudentProfileView.vue'
export { useStudentProfile } from './composables/useStudentProfile'
export { studentProfileService } from './services/studentProfile.service'

export type {
  StudentProfile,
  UpdateStudentProfilePayload,
  ChangeStudentPasswordPayload,
  ChangePasswordResponse
} from './types'