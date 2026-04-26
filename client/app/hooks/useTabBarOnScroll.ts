import { useContext } from "react";
import { useAnimatedScrollHandler, useSharedValue, withTiming } from "react-native-reanimated";
import { TabBarVisibilityContext } from "../components/bottomTabWrapper";

export const useHideTabBarOnScroll = () => {
  const context = useContext(TabBarVisibilityContext);

  if (!context) {
    throw new Error(
      "useHideTabBarOnScroll must be used within a TabBarVisibilityContext.Provider"
    );
  }

  const { tabBarTranslateY, tabBarHiddenOffset } = context;
  const lastScrollY = useSharedValue(0);
  const isHidden = useSharedValue(false);
  const hideThreshold = 24;
  const showThreshold = 12;

  const onScroll = useAnimatedScrollHandler({
    onScroll: (event) => {
      const currentOffset = event.contentOffset.y;
      const delta = currentOffset - lastScrollY.value;

      if (currentOffset <= 0 && isHidden.value) {
        isHidden.value = false;
        tabBarTranslateY.value = withTiming(0, { duration: 140 });
      } else if (delta > hideThreshold && !isHidden.value) {
        isHidden.value = true;
        tabBarTranslateY.value = withTiming(tabBarHiddenOffset, { duration: 140 });
      } else if (delta < -showThreshold && isHidden.value) {
        isHidden.value = false;
        tabBarTranslateY.value = withTiming(0, { duration: 140 });
      }

      lastScrollY.value = currentOffset;
    },
  });

  return { onScroll };
};