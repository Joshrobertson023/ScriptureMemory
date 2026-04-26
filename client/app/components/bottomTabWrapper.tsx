import React, { createContext, ReactNode, useMemo } from "react";
import { useSharedValue, SharedValue } from "react-native-reanimated";

interface TabBarContextType {
  tabBarTranslateY: SharedValue<number>;
  tabBarHiddenOffset: number;
}

export const TabBarVisibilityContext = createContext<
  TabBarContextType | undefined
>(undefined);

interface TabBarProviderProps {
  children: ReactNode;
}

export const BottomTabWrapper: React.FC<TabBarProviderProps> = ({
  children,
}) => {
  const tabBarTranslateY = useSharedValue(0);
  const tabBarHiddenOffset = 120;

  const value = useMemo(
    () => ({ tabBarTranslateY, tabBarHiddenOffset }),
    [tabBarTranslateY]
  );

  return (
    <TabBarVisibilityContext.Provider value={value}>
      {children}
    </TabBarVisibilityContext.Provider>
  );
};