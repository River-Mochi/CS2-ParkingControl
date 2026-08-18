// File: UI/src/index.tsx
// Purpose: Registers Parking Control UI extensions.

import type { ModRegistrar } from "cs2/modding";
import { registerDistrictPolicyFocusFix } from "./extensions/districtPolicyFocus";

const register: ModRegistrar = (moduleRegistry) => {
    registerDistrictPolicyFocusFix(moduleRegistry);
};

export default register;
