import { View, Text, StyleSheet } from "react-native";
import { Category } from "../../../types/category"
import { useEffect, useMemo, useState } from "react";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

interface CategoriesProps {
    categories: Category[],
    multiline: boolean;
}


const Categories = ({categories, multiline}: CategoriesProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        container: {
            flexDirection: 'row', marginBottom: 20
        },
        innerContainer: {
            flexDirection: 'row', gap: 10
        },
        categoryName: {
            display: 'flex', justifyContent: 'center', alignItems: 'center',
                                backgroundColor: theme.colors.elevation, borderRadius: 30, paddingHorizontal: 15
                            },
                            twoCategoryName: {
                                display: 'flex', justifyContent: 'center', alignItems: 'center',
                                backgroundColor: theme.colors.elevation, borderRadius: 30, paddingHorizontal: 15, paddingVertical: 2
                            },
                            categoriesExtra: {
            display: 'flex', justifyContent: 'center', alignItems: 'center',
            backgroundColor: theme.colors.elevation, borderRadius: 30
        }
    }), [theme])
    const styles = useLocalStyles();

    const [firstTwoCategories, setFirstTwoCategories] = useState<Category[]>([]);

    useEffect(() => {
        setFirstTwoCategories(categories.slice(0, 2));
    }, [categories]);

    return (
        <View style={styles.container}>
            {multiline ? (
                <View style={styles.innerContainer}>
                    {categories.map((category, index) => {
                        return (
                            <View style={styles.categoryName}>
                                <Text style={globalStyles.p4}>{category.name}</Text>
                            </View>
                        )
                    })}
                </View>
            ) : (
                <View style={styles.innerContainer}>
                    {firstTwoCategories.map((category, index) => {
                        return (
                            <View style={styles.twoCategoryName}>
                                <Text style={globalStyles.p4}>{category.name}</Text>
                            </View>
                        )
                    })}
                    {categories.length > 2 && (
                        <View style={styles.categoriesExtra}>
                            <Text style={globalStyles.p4}>+{categories.length - 2}</Text>
                        </View>
                    )}
                </View>
            )}
        </View>
    )
}

export default Categories;