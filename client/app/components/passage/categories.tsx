import { View, Text } from "react-native";
import { Category } from "../../../types/category"
import { useEffect, useState } from "react";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

interface CategoriesProps {
    categories: Category[],
    multiline: boolean;
}

const Categories = ({categories, multiline}: CategoriesProps) => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();

    const [firstTwoCategories, setFirstTwoCategories] = useState<Category[]>([]);

    useEffect(() => {
        setFirstTwoCategories(categories.slice(0, 2));
    }, [categories]);

    return (
        <View style={{flexDirection: 'column', justifyContent: 'center', alignItems: 'center'}}>
            {multiline ? (
                categories.map((category, index) => {
                    return (
                        <View style={{display: 'flex', justifyContent: 'center', alignItems: 'center',
                            backgroundColor: theme.colors.elevation, borderRadius: 30, paddingHorizontal: 15
                        }}>
                            <Text style={styles.p4}>{category.name}</Text>
                        </View>
                    )
                })
            ) : (
                <View>
                    {firstTwoCategories.map((category, index) => {
                        return (
                            <View style={{display: 'flex', justifyContent: 'center', alignItems: 'center',
                                backgroundColor: theme.colors.elevation, borderRadius: 30, paddingHorizontal: 15, paddingVertical: 2
                            }}>
                                <Text style={styles.p4}>{category.name}</Text>
                            </View>
                        )
                    })}
                    {categories.length > 2 && (
                        <View style={{display: 'flex', justifyContent: 'center', alignItems: 'center',
                            backgroundColor: theme.colors.elevation, borderRadius: 30
                        }}>
                            <Text style={styles.p4}>+{categories.length - 2}</Text>
                        </View>
                    )}
                </View>
            )}
        </View>
    )
}

export default Categories;