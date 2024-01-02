/*! firebase-admin v11.0.0 */
/*!
 * Copyright 2019 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
import { EmailSignInConfigServerRequest, MultiFactorAuthServerConfig, MultiFactorConfig, EmailSignInProviderConfig } from './auth-config';
/**
 * Interface representing the properties to update on the provided tenant.
 */
export interface UpdateTenantRequest {
    /**
     * The tenant display name.
     */
    displayName?: string;
    /**
     * The email sign in configuration.
     */
    emailSignInConfig?: EmailSignInProviderConfig;
    /**
     * Whether the anonymous provider is enabled.
     */
    anonymousSignInEnabled?: boolean;
    /**
     * The multi-factor auth configuration to update on the tenant.
     */
    multiFactorConfig?: MultiFactorConfig;
    /**
     * The updated map containing the test phone number / code pairs for the tenant.
     * Passing null clears the previously save phone number / code pairs.
     */
    testPhoneNumbers?: {
        [phoneNumber: string]: string;
    } | null;
}
/**
 * Interface representing the properties to set on a new tenant.
 */
export declare type CreateTenantRequest = UpdateTenantRequest;
/** The corresponding server side representation of a TenantOptions object. */
export interface TenantOptionsServerRequest extends EmailSignInConfigServerRequest {
    displayName?: string;
    enableAnonymousUser?: boolean;
    mfaConfig?: MultiFactorAuthServerConfig;
    testPhoneNumbers?: {
        [key: string]: string;
    };
}
/** The tenant server response interface. */
export interface TenantServerResponse {
    name: string;
    displayName?: string;
    allowPasswordSignup?: boolean;
    enableEmailLinkSignin?: boolean;
    enableAnonymousUser?: boolean;
    mfaConfig?: MultiFactorAuthServerConfig;
    testPhoneNumbers?: {
        [key: string]: string;
    };
}
/**
 * Represents a tenant configuration.
 *
 * Multi-tenancy support requires Google Cloud's Identity Platform
 * (GCIP). To learn more about GCIP, including pricing and features,
 * see the {@link https://cloud.google.com/identity-platform | GCIP documentation}.
 *
 * Before multi-tenancy can be used on a Google Cloud Identity Platform project,
 * tenants must be allowed on that project via the Cloud Console UI.
 *
 * A tenant configuration provides information such as the display name, tenant
 * identifier and email authentication configuration.
 * For OIDC/SAML provider configuration management, `TenantAwareAuth` instances should
 * be used instead of a `Tenant` to retrieve the list of configured IdPs on a tenant.
 * When configuring these providers, note that tenants will inherit
 * whitelisted domains and authenticated redirect URIs of their parent project.
 *
 * All other settings of a tenant will also be inherited. These will need to be managed
 * from the Cloud Console UI.
 */
export declare class Tenant {
    /**
     * The tenant identifier.
     */
    readonly tenantId: string;
    /**
     * The tenant display name.
     */
    readonly displayName?: string;
    readonly anonymousSignInEnabled: boolean;
    /**
     * The map containing the test phone number / code pairs for the tenant.
     */
    readonly testPhoneNumbers?: {
        [phoneNumber: string]: string;
    };
    private readonly emailSignInConfig_?;
    private readonly multiFactorConfig_?;
    /**
     * Validates a tenant options object. Throws an error on failure.
     *
     * @param request - The tenant options object to validate.
     * @param createRequest - Whether this is a create request.
     */
    private static validate;
    /**
     * The email sign in provider configuration.
     */
    get emailSignInConfig(): EmailSignInProviderConfig | undefined;
    /**
     * The multi-factor auth configuration on the current tenant.
     */
    get multiFactorConfig(): MultiFactorConfig | undefined;
    /**
     * Returns a JSON-serializable representation of this object.
     *
     * @returns A JSON-serializable representation of this object.
     */
    toJSON(): object;
}