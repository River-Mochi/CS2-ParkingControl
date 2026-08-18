// File: UI/src/diagnostics/policyFocusDiagnostics.tsx
// Purpose: One-run diagnostics for the district policy focus-registration error.

import {
    createContext,
    useContext,
} from "react";

import {
    ActiveFocusDiv,
    AutoNavigationScope,
    FocusBoundary,
    FocusDisabled,
} from "cs2/input";

import type {
    ModuleRegistry,
    ModuleRegistryExtend,
} from "cs2/modding";

import {
    PanelSectionRow,
} from "cs2/ui";

const POLICY_MODULE =
    "game-ui/game/components/policy/policy.tsx";

const INFO_ROW_MODULE =
    "game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.tsx";

const PC_POLICY_ID = "PCDistrictParkingBan";

const kSamplePolicyIds = new Set([
    PC_POLICY_ID,
    "Heavy Traffic Ban",
    "Recycling",
    "Bicycle Traffic Restriction",
]);

const PolicyIdContext =
    createContext<string | null>(null);

const s_SeenPolicies = new Set<string>();
const s_SeenInfoRows = new Set<string>();

function safeJson(value: unknown): string {
    const seen = new WeakSet<object>();

    try {
        return JSON.stringify(
            value,
            (_key, item) => {
                if (
                    typeof item === "object" &&
                    item !== null
                ) {
                    if (seen.has(item)) {
                        return "[Circular]";
                    }

                    seen.add(item);
                }

                if (typeof item === "function") {
                    return `[Function ${item.name || "anonymous"}]`;
                }

                return item;
            }
        );
    } catch (error) {
        return `[unserializable: ${String(error)}]`;
    }
}

function getSafe(
    moduleRegistry: ModuleRegistry,
    path: string,
    exportName: string
): any {
    try {
        return moduleRegistry.get(
            path,
            exportName
        );
    } catch (error) {
        console.error(
            `[ParkingControl][UIDIAG][GET] ${path}#${exportName}`,
            error
        );

        return undefined;
    }
}

function logIdentity(
    moduleRegistry: ModuleRegistry,
    label: string,
    target: any
): void {
    const matches: string[] = [];

    for (
        const [path, exports]
        of moduleRegistry.registry.entries()
    ) {
        for (
            const [exportName, value]
            of Object.entries(exports)
        ) {
            if (value === target) {
                matches.push(
                    `${path}#${exportName}`
                );
            }
        }
    }

    console.log(
        `[ParkingControl][UIDIAG][IDENTITY] ${label} :: ` +
        (matches.length > 0
            ? matches.join(" | ")
            : "<none>")
    );
}

function logSource(
    label: string,
    value: any
): void {
    if (typeof value !== "function") {
        console.log(
            `[ParkingControl][UIDIAG][SOURCE] ${label} :: ` +
            `type=${typeof value}, keys=` +
            `${Object.keys(value ?? {}).join(",")}`
        );

        return;
    }

    try {
        const source =
            Function.prototype.toString
                .call(value)
                .replace(/\s+/g, " ");

        console.log(
            `[ParkingControl][UIDIAG][SOURCE] ${label} :: ` +
            source.slice(0, 5000)
        );
    } catch (error) {
        console.error(
            `[ParkingControl][UIDIAG][SOURCE] ${label}`,
            error
        );
    }
}

function dumpRelevantRegistry(
    moduleRegistry: ModuleRegistry
): void {
    const relevant =
        /policy|info[-_]?row|info[-_]?section|focus|navigation|checkbox|toggle|selected[-_]?info/i;

    for (
        const [path, exports]
        of moduleRegistry.registry.entries()
    ) {
        const exportNames =
            Object.keys(exports);

        if (
            !relevant.test(path) &&
            !exportNames.some(
                name => relevant.test(name)
            )
        ) {
            continue;
        }

        console.log(
            `[ParkingControl][UIDIAG][MODULE] ${path} :: ` +
            exportNames.join(", ")
        );
    }
}

const PolicyProbeExtension:
    ModuleRegistryExtend = (Component) => {
        return (props: any) => {
            const id =
                props?.policy?.id ?? null;

            if (
                typeof id === "string" &&
                kSamplePolicyIds.has(id) &&
                !s_SeenPolicies.has(id)
            ) {
                s_SeenPolicies.add(id);

                console.log(
                    `[ParkingControl][UIDIAG][POLICY] ${id} :: ` +
                    safeJson(props?.policy)
                );
            }

            return (
                <PolicyIdContext.Provider value={id}>
                    <Component {...props} />
                </PolicyIdContext.Provider>
            );
        };
    };

const InfoRowProbeExtension:
    ModuleRegistryExtend = (Component) => {
        return (props: any) => {
            const policyId =
                useContext(PolicyIdContext);

            if (
                policyId !== null &&
                kSamplePolicyIds.has(policyId) &&
                !s_SeenInfoRows.has(policyId)
            ) {
                s_SeenInfoRows.add(policyId);

                console.log(
                    `[ParkingControl][UIDIAG][INFOROW] ${policyId} :: ` +
                    `keys=${Object.keys(props ?? {}).join(",")} :: ` +
                    safeJson(props)
                );
            }

            return <Component {...props} />;
        };
    };

export function registerPolicyFocusDiagnostics(
    moduleRegistry: ModuleRegistry
): void {
    console.log(
        "[ParkingControl][UIDIAG] ===== BEGIN ====="
    );

    const policyBefore =
        getSafe(
            moduleRegistry,
            POLICY_MODULE,
            "Policy"
        );

    const compactPolicyBefore =
        getSafe(
            moduleRegistry,
            POLICY_MODULE,
            "CompactPolicy"
        );

    const infoRowBefore =
        getSafe(
            moduleRegistry,
            INFO_ROW_MODULE,
            "InfoRow"
        );

    console.log(
        `[ParkingControl][UIDIAG][COMPARE] ` +
        `PanelSectionRow===InfoRowBefore :: ` +
        `${PanelSectionRow === infoRowBefore}`
    );

    logIdentity(
        moduleRegistry,
        "PanelSectionRow",
        PanelSectionRow
    );

    logIdentity(
        moduleRegistry,
        "InfoRowBefore",
        infoRowBefore
    );

    logIdentity(
        moduleRegistry,
        "Policy",
        policyBefore
    );

    logIdentity(
        moduleRegistry,
        "CompactPolicy",
        compactPolicyBefore
    );

    logIdentity(
        moduleRegistry,
        "ActiveFocusDiv",
        ActiveFocusDiv
    );

    logIdentity(
        moduleRegistry,
        "AutoNavigationScope",
        AutoNavigationScope
    );

    logIdentity(
        moduleRegistry,
        "FocusBoundary",
        FocusBoundary
    );

    logIdentity(
        moduleRegistry,
        "FocusDisabled",
        FocusDisabled
    );

    logSource(
        "PanelSectionRow",
        PanelSectionRow
    );

    logSource(
        "InfoRowBefore",
        infoRowBefore
    );

    logSource(
        "Policy",
        policyBefore
    );

    logSource(
        "CompactPolicy",
        compactPolicyBefore
    );

    dumpRelevantRegistry(
        moduleRegistry
    );

    try {
        moduleRegistry.extend(
            POLICY_MODULE,
            "Policy",
            PolicyProbeExtension
        );
    } catch (error) {
        console.error(
            "[ParkingControl][UIDIAG] Policy probe extension failed",
            error
        );
    }

    try {
        moduleRegistry.extend(
            INFO_ROW_MODULE,
            "InfoRow",
            InfoRowProbeExtension
        );
    } catch (error) {
        console.error(
            "[ParkingControl][UIDIAG] InfoRow probe extension failed",
            error
        );
    }

    const infoRowAfter =
        getSafe(
            moduleRegistry,
            INFO_ROW_MODULE,
            "InfoRow"
        );

    console.log(
        `[ParkingControl][UIDIAG][COMPARE] ` +
        `InfoRowBefore===InfoRowAfter :: ` +
        `${infoRowBefore === infoRowAfter}`
    );

    console.log(
        `[ParkingControl][UIDIAG][COMPARE] ` +
        `PanelSectionRow===InfoRowAfter :: ` +
        `${PanelSectionRow === infoRowAfter}`
    );

    logIdentity(
        moduleRegistry,
        "InfoRowAfter",
        infoRowAfter
    );

    console.log(
        "[ParkingControl][UIDIAG] ===== REGISTERED ====="
    );
}
