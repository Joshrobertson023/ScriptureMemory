import { HelperText, TextInput } from "react-native-paper";
import useStyles from "../../styles";
import { useState } from "react";
import { useAppStore } from "../../store";

interface UsernameInputProps {
    errorMessage: string;
    setErrorMessage: (message: string) => void;
}

export const UsernameInput = ({errorMessage, setErrorMessage}: UsernameInputProps) => {
    const styles = useStyles();
    const [usernameEmpty, setUsernameEmtpy] = useState(false);
    const username = useAppStore(s => s.loginInfo.username);
    const loginInfo = useAppStore(s => s.loginInfo);
    const setLoginInfo = useAppStore(s => s.setLoginInfo);

    return (
        <>
            <TextInput style={{...styles.input}} error={usernameEmpty} value={username} 
                onChangeText={ (text) => {
                    setLoginInfo({...loginInfo, username});
                    if (errorMessage) setErrorMessage('');
                    if (text) setUsernameEmtpy(false);
                }
                } />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={usernameEmpty}>
                Create a username
            </HelperText>
        </>
    )
}