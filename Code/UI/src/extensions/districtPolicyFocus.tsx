// File: UI/src/extensions/districtPolicyFocus.tsx
// Purpose: Identifies vanilla expanded policy-row props before applying the focus fix.

import type {
  ModuleRegistry,
  ModuleRegistryExtend,
} from "cs2/modding";

const POLICY_MODULE =
  "game-ui/game/components/policy/policy.tsx";

const POLICY_EXPORT = "Policy";

const s_Seen = new Set<string>();

const PolicyProbeExtension: ModuleRegistryExtend = (Component) => {
  return (props: any) => {
    const policy = props?.policy ?? props?.item ?? props?.data;

    const summary = {
      topKeys: Object.keys(props ?? {}),
      policyKeys:
        policy && typeof policy === "object"
          ? Object.keys(policy)
          : [],
      topId: props?.id ?? null,
      policyId: policy?.id ?? null,
      uiTag: policy?.uiTag ?? props?.uiTag ?? null,
      localizedName:
        policy?.localizedName ??
        props?.localizedName ??
        null,
    };

    const key = JSON.stringify(summary);

    if (!s_Seen.has(key)) {
      s_Seen.add(key);
      console.log(
        `[ParkingControl][PolicyProbe] ${key}`
      );
    }

    return <Component {...props} />;
  };
};

export function registerDistrictPolicyFocusProbe(
  moduleRegistry: ModuleRegistry
): void {
  moduleRegistry.extend(
    POLICY_MODULE,
    POLICY_EXPORT,
    PolicyProbeExtension
  );
}
