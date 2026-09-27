import { LinkButton } from "heroui-native";
import React from "react";
import { Linking, Text, View } from "react-native";
import useGlobalStyles from "../styles/gobalStyles";
import useAppTheme from "../theme";

export const CreditsScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();

    return (
        <View style={styles.screen}>
            <Text style={styles.p1}>
                Credits
            </Text>
            <View style={{height: 25}} />
            <Text style={styles.p2}>Cross References:</Text>
            <LinkButton onPress={() => {Linking.openURL('https://www.openbible.info/labs/cross-references/')}}>OpenBible</LinkButton>
            <View style={{height: 25}} />
            <Text style={styles.p2}>Verse text (except for KJV), Bible formatting, section titles, and copyrights:</Text>
            <LinkButton onPress={() => {Linking.openURL('https://api.bible/')}}>API.Bible</LinkButton>
        </View>
    )
}