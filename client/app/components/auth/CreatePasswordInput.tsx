import { HelperText, TextInput } from "react-native-paper";
import useStyles from "../../styles";
import { useFormStore } from "../../stores/form.store";
import { useState } from "react";

export const CreatePasswordInput = () => {
    const styles = useStyles();
    const password = useFormStore(s => s.registerForm.password);
    const confirmPassword = useFormStore(s => s.registerForm.confirmPassword);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);
    const updateForm = useFormStore(s => s.updateRegister);
    const [passwordEmpty, setPasswordEmpty] = useState(false);
    const [confirmEmpty, setConfirmEmpty] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [passwordTooShort, setPasswordTooShort] = useState(false);

    return (
        <>
            <TextInput keyboardType="default"
                autoCapitalize="none"
                autoCorrect={false}
                autoComplete="password"
                textContentType="password"
                secureTextEntry={!showPassword}
                error={passwordEmpty}
                label="Password" mode="outlined" style={styles.input} value={password}
                right={<TextInput.Icon icon={showPassword ? 'eye-off' : 'eye'} onPress={() => setShowPassword((prev) => !prev)} />}
                onChangeText={(text) => {
                    const trimmed = text.trim();
                    updateForm({password: trimmed})
                    if (text) setPasswordEmpty(false);
                    if (trimmed.length > 0 && trimmed.length < 11)
                        setPasswordTooShort(true);
                    else 
                        setPasswordTooShort(false);
                }} 
                maxLength={30}/>
            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={passwordEmpty}>
                Enter your password
            </HelperText>
            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -5, marginBottom: -5, height: 25}} type="error" visible={passwordTooShort}>
                Password must be at least 11 characters long.
            </HelperText>
            <TextInput keyboardType="default"
                autoCapitalize="none"
                autoCorrect={false}
                autoComplete="password"
                textContentType="password"
                secureTextEntry={!showConfirmPassword}
                error={confirmEmpty}
                label="Confirm Password" mode="outlined" style={styles.input} value={confirmPassword} 
                right={<TextInput.Icon icon={showConfirmPassword ? 'eye-off' : 'eye'} onPress={() => setShowConfirmPassword((prev) => !prev)} />}
                onChangeText={(text) => updateForm({confirmPassword: text}) }
                maxLength={30}/>
            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={confirmEmpty}>
                Confirm your password
            </HelperText>
        </>
    )
}