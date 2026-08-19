// File: UI/src/index.tsx
// Purpose: Registers Parking Control UI assets.

import type { ModRegistrar } from "cs2/modding";

// Ensure custom icons are emitted to coui://ui-mods/images/.
import "../images/PC-DistrictParkingBan.svg";
import "../images/ForbidParking.svg";

const register: ModRegistrar = () => {
};

export default register;
