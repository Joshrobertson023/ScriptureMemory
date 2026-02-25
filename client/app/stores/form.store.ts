import { create } from "zustand";
import { generateUsername } from "../utils/loginUtils";
import { BibleVersion } from "../../types/enums";

export interface RegisterForm {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  bibleVersion: BibleVersion;
  errorMessage: string;
  errors: Record<string, string>;
}

export interface LoginForm {
  username: string;
  password: string;
  errorMessage: string;
  errors: Record<string, string>;
  usernameIncorrect: boolean;
  passwordIncorrect: boolean;
}

interface FormStore {
  registerForm: RegisterForm;
  loginForm: LoginForm;

  updateRegister: (updates: Partial<RegisterForm>) => void;
  updateLogin: (updates: Partial<LoginForm>) => void;

  resetRegister: () => void;
  resetLogin: () => void;
}

const initialRegister: RegisterForm = {
  firstName: '',
  lastName: '',
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
  bibleVersion: 0,
  errorMessage: '',
  errors: {}
};

const initialLogin: LoginForm = {
  username: '',
  password: '',
  errorMessage: '',
  errors: {},
  usernameIncorrect: false,
  passwordIncorrect: false,
};

export const useFormStore = create<FormStore>((set, get) => ({
  registerForm: initialRegister,
  loginForm: initialLogin,

  updateRegister: (updates) =>
    set((state) => {
      const newForm = { ...state.registerForm, ...updates };

      if ((newForm.firstName && newForm.lastName) 
        && (updates.firstName || updates.lastName))
        newForm.username = generateUsername(newForm.firstName, newForm.lastName);

      return {registerForm: newForm}
    }),

  updateLogin: (updates) =>
    set((state) => ({
      loginForm: { ...state.loginForm, ...updates }
    })),

  resetRegister: () => set({ registerForm: initialRegister }),
  resetLogin: () => set({ loginForm: initialLogin }),
}));