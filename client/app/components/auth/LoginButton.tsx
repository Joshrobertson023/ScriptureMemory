import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import useStyles from "../../styles";

export const LoginButton = () => {
    const styles = useStyles();
    
    return (
        <>
            <Text style={{...styles.tinyText, marginTop: 20}}>Already have an account?</Text>
            {/* <Link href="/(auth)/login" style={{marginTop: 0, paddingVertical: 10}}>
                <Text style={{...styles.tinyText, color: theme.colors.primary, textDecorationLine: 'underline'}}>Log In</Text>
            </Link> */}
        </>
    )
}