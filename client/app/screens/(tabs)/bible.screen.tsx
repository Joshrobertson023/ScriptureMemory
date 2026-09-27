import { createNativeStackNavigator } from "@react-navigation/native-stack";
import * as React from "react";
import { RootStackParamList } from "../../../types/router";
import useStyles from '../../styles/gobalStyles';
import useAppTheme from "../../theme";
import ChooseBookScreen from "../bible/chooseBook.screen";
import ChooseChapterScreen from "../bible/chooseChapter.screen";
import ReadScreen from "../bible/read.screen";

const Stack = createNativeStackNavigator<RootStackParamList>();

export const BibleScreen = () => {
    const styles = useStyles();
    const theme = useAppTheme();
    
    return (
        <Stack.Navigator>
            <Stack.Screen
                name="chooseBook"
                component={ChooseBookScreen}
                options={{
                    headerShown: false
                }}
            />
            <Stack.Screen
                name="chooseChapter"
                component={ChooseChapterScreen}
                options={{
                    headerShown: true,
                    headerStyle: {
                        backgroundColor: theme.colors.background2
                    },
                    headerTitleStyle: {
                        color: theme.colors.onBackground
                    },
                    headerTintColor: theme.colors.onBackground
                }}
            />
            <Stack.Screen
                name="read"
                component={ReadScreen}
                options={{
                    headerShown: false
                }}
            />
        </Stack.Navigator>
    )
}