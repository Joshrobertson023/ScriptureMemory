import { create } from "zustand";

interface Form {
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
}

interface RegisterForm {

    form: {
        firstName: string;
        lastName: string;
        username: string;
        email: string;
        password: string;
        confirmPassword: string;
    };
    errors: Record<string, string>;
    errorMessage: string;

    setForm: (f: Form) => void;
    setErrorMessage: (err: string) => void;
    setErrors: (errors: Record<string, string>) => void;
}

export const useFormStore = create<RegisterForm>((set) => ({
    form: {
        firstName: '',
        lastName: '',
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
    },
    errorMessage: '',
    errors: {},

    setForm: (form: Form) => set({form}),
    setErrorMessage: (msg: string) => set({errorMessage: msg}),
    setErrors: (errors: Record<string, string>) => set({errors})
}))