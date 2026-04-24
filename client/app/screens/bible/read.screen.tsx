import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { View } from "react-native";
import { RootStackParamList } from "../../../types/router";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";

type Props = NativeStackScreenProps<RootStackParamList, 'read'>;

const ReadScreen = ({route}: Props) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const {book, chapter} = route.params;

    return (
        <View>

        </View>
    )
}

export default ReadScreen;