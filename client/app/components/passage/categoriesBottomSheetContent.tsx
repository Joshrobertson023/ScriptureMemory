import { Text, View, StyleSheet, TouchableWithoutFeedback } from "react-native";
import { useMemo } from "react";
import { Category } from "../../../types/category";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

interface CategoriesBottomSheetContentProps {
    categories: Category[];
    multiline: boolean;
}

const CategoriesBottomSheetContent = ({ categories, multiline }: CategoriesBottomSheetContentProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const { setCategoriesBottomSheet, setCategoriesSheetOpen } = useBottomSheetsStore();

    const styles = useMemo(() => StyleSheet.create({
        container: {
            flexDirection: 'row',
            marginBottom: 20,
        },
        innerContainer: {
            flexDirection: 'row',
            gap: 10,
            flexWrap: 'wrap',
        },
        categoryName: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            backgroundColor: theme.colors.elevation,
            borderRadius: 30,
            paddingHorizontal: 15,
            paddingVertical: 5
        },
        twoCategoryName: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            backgroundColor: theme.colors.elevation,
            borderRadius: 30,
            paddingHorizontal: 15,
            paddingVertical: 2,
        },
        categoriesExtra: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            backgroundColor: theme.colors.elevation,
            borderRadius: 30,
            paddingHorizontal: 12,
        },
    }), [theme]);

    const firstTwoCategories = useMemo(() => categories.slice(0, 2), [categories]);

    const handleCategoryPress = (category: Category) => {
        setCategoriesBottomSheet(category);
        setCategoriesSheetOpen(true);
    };

    return (
        <View style={styles.container}>
            {multiline ? (
                <View style={styles.innerContainer}>
                    {categories.map((category) => (
                        <TouchableWithoutFeedback key={category.id} onPress={() => handleCategoryPress(category)}>
                            <View style={styles.categoryName}>
                                <Text style={globalStyles.p3}>{category.name}</Text>
                            </View>
                        </TouchableWithoutFeedback>
                    ))}
                </View>
            ) : (
                <View style={styles.innerContainer}>
                    {firstTwoCategories.map((category) => (
                        <TouchableWithoutFeedback key={category.id} onPress={() => handleCategoryPress(category)}>
                            <View style={styles.twoCategoryName}>
                                <Text style={globalStyles.p3}>{category.name}</Text>
                            </View>
                        </TouchableWithoutFeedback>
                    ))}
                    {categories.length > 2 && (
                        <View style={styles.categoriesExtra}>
                            <Text style={globalStyles.p3}>+{categories.length - 2}</Text>
                        </View>
                    )}
                </View>
            )}
        </View>
    );
};

export default CategoriesBottomSheetContent;
