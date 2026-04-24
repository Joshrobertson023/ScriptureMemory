import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { Text, View, StyleSheet } from "react-native";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

const CategoriesBottomSheet = forwardRef<TrueSheet>((_, ref) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const { categoriesBottomSheet, setCategoriesSheetOpen, clearCategoriesBottomSheet } = useBottomSheetsStore();

    const styles = StyleSheet.create({
        container: {
            flex: 1,
            padding: 20,
            paddingTop: 28,
            gap: 12,
        },
        title: {
            color: theme.colors.onBackground,
        },
        subtitle: {
            color: theme.colors.onBackgroundSoft,
        },
        card: {
            backgroundColor: theme.colors.elevation,
            borderRadius: 16,
            padding: 16,
            gap: 8,
        },
    });

    return (
        <TrueSheet
            ref={ref}
            detents={[0.35, 0.6]}
            onDidDismiss={() => {
                setCategoriesSheetOpen(false);
                clearCategoriesBottomSheet();
            }}
            style={{ backgroundColor: theme.colors.background }}
            scrollable
        >
            <View style={styles.container}>
                <Text style={[globalStyles.p2, styles.title]}>
                    Category
                </Text>
                {categoriesBottomSheet ? (
                    <View style={styles.card}>
                        <Text style={[globalStyles.p1, styles.title]}>
                            {categoriesBottomSheet.name}
                        </Text>
                        <Text style={[globalStyles.p4, styles.subtitle]}>
                            Category ID: {categoriesBottomSheet.id}
                        </Text>
                    </View>
                ) : (
                    <Text style={[globalStyles.p3, styles.subtitle]}>
                        No category selected.
                    </Text>
                )}
            </View>
        </TrueSheet>
    );
});

export default CategoriesBottomSheet;
