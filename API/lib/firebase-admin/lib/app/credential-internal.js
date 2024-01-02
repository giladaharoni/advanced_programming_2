/*! firebase-admin v11.0.0 */
"use strict";
/*!
 * @license
 * Copyright 2020 Google Inc.
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
Object.defineProperty(exports, "__esModule", { value: true });
exports.getApplicationDefault = exports.isApplicationDefault = exports.RefreshTokenCredential = exports.ComputeEngineCredential = exports.ServiceAccountCredential = void 0;
const fs = require("fs");
const os = require("os");
const path = require("path");
const error_1 = require("../utils/error");
const api_request_1 = require("../utils/api-request");
const util = require("../utils/validator");
const GOOGLE_TOKEN_AUDIENCE = 'https://accounts.google.com/o/oauth2/token';
const GOOGLE_AUTH_TOKEN_HOST = 'accounts.google.com';
const GOOGLE_AUTH_TOKEN_PATH = '/o/oauth2/token';
// NOTE: the Google Metadata Service uses HTTP over a vlan
const GOOGLE_METADATA_SERVICE_HOST = 'metadata.google.internal';
const GOOGLE_METADATA_SERVICE_TOKEN_PATH = '/computeMetadata/v1/instance/service-accounts/default/token';
const GOOGLE_METADATA_SERVICE_PROJECT_ID_PATH = '/computeMetadata/v1/project/project-id';
const GOOGLE_METADATA_SERVICE_ACCOUNT_ID_PATH = '/computeMetadata/v1/instance/service-accounts/default/email';
const configDir = (() => {
    // Windows has a dedicated low-rights location for apps at ~/Application Data
    const sys = os.platform();
    if (sys && sys.length >= 3 && sys.substring(0, 3).toLowerCase() === 'win') {
        return process.env.APPDATA;
    }
    // On *nix the gcloud cli creates a . dir.
    return process.env.HOME && path.resolve(process.env.HOME, '.config');
})();
const GCLOUD_CREDENTIAL_SUFFIX = 'gcloud/application_default_credentials.json';
const GCLOUD_CREDENTIAL_PATH = configDir && path.resolve(configDir, GCLOUD_CREDENTIAL_SUFFIX);
const REFRESH_TOKEN_HOST = 'www.googleapis.com';
const REFRESH_TOKEN_PATH = '/oauth2/v4/token';
const ONE_HOUR_IN_SECONDS = 60 * 60;
const JWT_ALGORITHM = 'RS256';
/**
 * Implementation of Credential that uses a service account.
 */
class ServiceAccountCredential {
    /**
     * Creates a new ServiceAccountCredential from the given parameters.
     *
     * @param serviceAccountPathOrObject - Service account json object or path to a service account json file.
     * @param httpAgent - Optional http.Agent to use when calling the remote token server.
     * @param implicit - An optinal boolean indicating whether this credential was implicitly discovered from the
     *   environment, as opposed to being explicitly specified by the developer.
     *
     * @constructor
     */
    constructor(serviceAccountPathOrObject, httpAgent, implicit = false) {
        this.httpAgent = httpAgent;
        this.implicit = implicit;
        const serviceAccount = (typeof serviceAccountPathOrObject === 'string') ?
            ServiceAccount.fromPath(serviceAccountPathOrObject)
            : new ServiceAccount(serviceAccountPathOrObject);
        this.projectId = serviceAccount.projectId;
        this.privateKey = serviceAccount.privateKey;
        this.clientEmail = serviceAccount.clientEmail;
        this.httpClient = new api_request_1.HttpClient();
    }
    getAccessToken() {
        const token = this.createAuthJwt_();
        const postData = 'grant_type=urn%3Aietf%3Aparams%3Aoauth%3A' +
            'grant-type%3Ajwt-bearer&assertion=' + token;
        const request = {
            method: 'POST',
            url: `https://${GOOGLE_AUTH_TOKEN_HOST}${GOOGLE_AUTH_TOKEN_PATH}`,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            data: postData,
            httpAgent: this.httpAgent,
        };
        return requestAccessToken(this.httpClient, request);
    }
    // eslint-disable-next-line @typescript-eslint/naming-convention
    createAuthJwt_() {
        const claims = {
            scope: [
                'https://www.googleapis.com/auth/cloud-platform',
                'https://www.googleapis.com/auth/firebase.database',
                'https://www.googleapis.com/auth/firebase.messaging',
                'https://www.googleapis.com/auth/identitytoolkit',
                'https://www.googleapis.com/auth/userinfo.email',
            ].join(' '),
        };
        // eslint-disable-next-line @typescript-eslint/no-var-requires
        const jwt = require('jsonwebtoken');
        // This method is actually synchronous so we can capture and return the buffer.
        return jwt.sign(claims, this.privateKey, {
            audience: GOOGLE_TOKEN_AUDIENCE,
            expiresIn: ONE_HOUR_IN_SECONDS,
            issuer: this.clientEmail,
            algorithm: JWT_ALGORITHM,
        });
    }
}
exports.ServiceAccountCredential = ServiceAccountCredential;
/**
 * A struct containing the properties necessary to use service account JSON credentials.
 */
class ServiceAccount {
    constructor(json) {
        if (!util.isNonNullObject(json)) {
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Service account must be an object.');
        }
        copyAttr(this, json, 'projectId', 'project_id');
        copyAttr(this, json, 'privateKey', 'private_key');
        copyAttr(this, json, 'clientEmail', 'client_email');
        let errorMessage;
        if (!util.isNonEmptyString(this.projectId)) {
            errorMessage = 'Service account object must contain a string "project_id" property.';
        }
        else if (!util.isNonEmptyString(this.privateKey)) {
            errorMessage = 'Service account object must contain a string "private_key" property.';
        }
        else if (!util.isNonEmptyString(this.clientEmail)) {
            errorMessage = 'Service account object must contain a string "client_email" property.';
        }
        if (typeof errorMessage !== 'undefined') {
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, errorMessage);
        }
        // eslint-disable-next-line @typescript-eslint/no-var-requires
        const forge = require('node-forge');
        try {
            forge.pki.privateKeyFromPem(this.privateKey);
        }
        catch (error) {
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Failed to parse private key: ' + error);
        }
    }
    static fromPath(filePath) {
        try {
            return new ServiceAccount(JSON.parse(fs.readFileSync(filePath, 'utf8')));
        }
        catch (error) {
            // Throw a nicely formed error message if the file contents cannot be parsed
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Failed to parse service account json file: ' + error);
        }
    }
}
/**
 * Implementation of Credential that gets access tokens from the metadata service available
 * in the Google Cloud Platform. This authenticates the process as the default service account
 * of an App Engine instance or Google Compute Engine machine.
 */
class ComputeEngineCredential {
    constructor(httpAgent) {
        this.httpClient = new api_request_1.HttpClient();
        this.httpAgent = httpAgent;
    }
    getAccessToken() {
        const request = this.buildRequest(GOOGLE_METADATA_SERVICE_TOKEN_PATH);
        return requestAccessToken(this.httpClient, request);
    }
    getProjectId() {
        if (this.projectId) {
            return Promise.resolve(this.projectId);
        }
        const request = this.buildRequest(GOOGLE_METADATA_SERVICE_PROJECT_ID_PATH);
        return this.httpClient.send(request)
            .then((resp) => {
            this.projectId = resp.text;
            return this.projectId;
        })
            .catch((err) => {
            const detail = (err instanceof api_request_1.HttpError) ? getDetailFromResponse(err.response) : err.message;
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, `Failed to determine project ID: ${detail}`);
        });
    }
    getServiceAccountEmail() {
        if (this.accountId) {
            return Promise.resolve(this.accountId);
        }
        const request = this.buildRequest(GOOGLE_METADATA_SERVICE_ACCOUNT_ID_PATH);
        return this.httpClient.send(request)
            .then((resp) => {
            this.accountId = resp.text;
            return this.accountId;
        })
            .catch((err) => {
            const detail = (err instanceof api_request_1.HttpError) ? getDetailFromResponse(err.response) : err.message;
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, `Failed to determine service account email: ${detail}`);
        });
    }
    buildRequest(urlPath) {
        return {
            method: 'GET',
            url: `http://${GOOGLE_METADATA_SERVICE_HOST}${urlPath}`,
            headers: {
                'Metadata-Flavor': 'Google',
            },
            httpAgent: this.httpAgent,
        };
    }
}
exports.ComputeEngineCredential = ComputeEngineCredential;
/**
 * Implementation of Credential that gets access tokens from refresh tokens.
 */
class RefreshTokenCredential {
    /**
     * Creates a new RefreshTokenCredential from the given parameters.
     *
     * @param refreshTokenPathOrObject - Refresh token json object or path to a refresh token
     *   (user credentials) json file.
     * @param httpAgent - Optional http.Agent to use when calling the remote token server.
     * @param implicit - An optinal boolean indicating whether this credential was implicitly
     *   discovered from the environment, as opposed to being explicitly specified by the developer.
     *
     * @constructor
     */
    constructor(refreshTokenPathOrObject, httpAgent, implicit = false) {
        this.httpAgent = httpAgent;
        this.implicit = implicit;
        this.refreshToken = (typeof refreshTokenPathOrObject === 'string') ?
            RefreshToken.fromPath(refreshTokenPathOrObject)
            : new RefreshToken(refreshTokenPathOrObject);
        this.httpClient = new api_request_1.HttpClient();
    }
    getAccessToken() {
        const postData = 'client_id=' + this.refreshToken.clientId + '&' +
            'client_secret=' + this.refreshToken.clientSecret + '&' +
            'refresh_token=' + this.refreshToken.refreshToken + '&' +
            'grant_type=refresh_token';
        const request = {
            method: 'POST',
            url: `https://${REFRESH_TOKEN_HOST}${REFRESH_TOKEN_PATH}`,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            data: postData,
            httpAgent: this.httpAgent,
        };
        return requestAccessToken(this.httpClient, request);
    }
}
exports.RefreshTokenCredential = RefreshTokenCredential;
class RefreshToken {
    constructor(json) {
        copyAttr(this, json, 'clientId', 'client_id');
        copyAttr(this, json, 'clientSecret', 'client_secret');
        copyAttr(this, json, 'refreshToken', 'refresh_token');
        copyAttr(this, json, 'type', 'type');
        let errorMessage;
        if (!util.isNonEmptyString(this.clientId)) {
            errorMessage = 'Refresh token must contain a "client_id" property.';
        }
        else if (!util.isNonEmptyString(this.clientSecret)) {
            errorMessage = 'Refresh token must contain a "client_secret" property.';
        }
        else if (!util.isNonEmptyString(this.refreshToken)) {
            errorMessage = 'Refresh token must contain a "refresh_token" property.';
        }
        else if (!util.isNonEmptyString(this.type)) {
            errorMessage = 'Refresh token must contain a "type" property.';
        }
        if (typeof errorMessage !== 'undefined') {
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, errorMessage);
        }
    }
    /*
     * Tries to load a RefreshToken from a path. Throws if the path doesn't exist or the
     * data at the path is invalid.
     */
    static fromPath(filePath) {
        try {
            return new RefreshToken(JSON.parse(fs.readFileSync(filePath, 'utf8')));
        }
        catch (error) {
            // Throw a nicely formed error message if the file contents cannot be parsed
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Failed to parse refresh token file: ' + error);
        }
    }
}
/**
 * Checks if the given credential was loaded via the application default credentials mechanism. This
 * includes all ComputeEngineCredential instances, and the ServiceAccountCredential and RefreshTokenCredential
 * instances that were loaded from well-known files or environment variables, rather than being explicitly
 * instantiated.
 *
 * @param credential - The credential instance to check.
 */
function isApplicationDefault(credential) {
    return credential instanceof ComputeEngineCredential ||
        (credential instanceof ServiceAccountCredential && credential.implicit) ||
        (credential instanceof RefreshTokenCredential && credential.implicit);
}
exports.isApplicationDefault = isApplicationDefault;
function getApplicationDefault(httpAgent) {
    if (process.env.GOOGLE_APPLICATION_CREDENTIALS) {
        return credentialFromFile(process.env.GOOGLE_APPLICATION_CREDENTIALS, httpAgent);
    }
    // It is OK to not have this file. If it is present, it must be valid.
    if (GCLOUD_CREDENTIAL_PATH) {
        const refreshToken = readCredentialFile(GCLOUD_CREDENTIAL_PATH, true);
        if (refreshToken) {
            return new RefreshTokenCredential(refreshToken, httpAgent, true);
        }
    }
    return new ComputeEngineCredential(httpAgent);
}
exports.getApplicationDefault = getApplicationDefault;
/**
 * Copies the specified property from one object to another.
 *
 * If no property exists by the given "key", looks for a property identified by "alt", and copies it instead.
 * This can be used to implement behaviors such as "copy property myKey or my_key".
 *
 * @param to - Target object to copy the property into.
 * @param from - Source object to copy the property from.
 * @param key - Name of the property to copy.
 * @param alt - Alternative name of the property to copy.
 */
function copyAttr(to, from, key, alt) {
    const tmp = from[key] || from[alt];
    if (typeof tmp !== 'undefined') {
        to[key] = tmp;
    }
}
/**
 * Obtain a new OAuth2 token by making a remote service call.
 */
function requestAccessToken(client, request) {
    return client.send(request).then((resp) => {
        const json = resp.data;
        if (!json.access_token || !json.expires_in) {
            throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, `Unexpected response while fetching access token: ${JSON.stringify(json)}`);
        }
        return json;
    }).catch((err) => {
        throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, getErrorMessage(err));
    });
}
/**
 * Constructs a human-readable error message from the given Error.
 */
function getErrorMessage(err) {
    const detail = (err instanceof api_request_1.HttpError) ? getDetailFromResponse(err.response) : err.message;
    return `Error fetching access token: ${detail}`;
}
/**
 * Extracts details from the given HTTP error response, and returns a human-readable description. If
 * the response is JSON-formatted, looks up the error and error_description fields sent by the
 * Google Auth servers. Otherwise returns the entire response payload as the error detail.
 */
function getDetailFromResponse(response) {
    if (response.isJson() && response.data.error) {
        const json = response.data;
        let detail = json.error;
        if (json.error_description) {
            detail += ' (' + json.error_description + ')';
        }
        return detail;
    }
    return response.text || 'Missing error payload';
}
function credentialFromFile(filePath, httpAgent) {
    const credentialsFile = readCredentialFile(filePath);
    if (typeof credentialsFile !== 'object' || credentialsFile === null) {
        throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Failed to parse contents of the credentials file as an object');
    }
    if (credentialsFile.type === 'service_account') {
        return new ServiceAccountCredential(credentialsFile, httpAgent, true);
    }
    if (credentialsFile.type === 'authorized_user') {
        return new RefreshTokenCredential(credentialsFile, httpAgent, true);
    }
    throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Invalid contents in the credentials file');
}
function readCredentialFile(filePath, ignoreMissing) {
    let fileText;
    try {
        fileText = fs.readFileSync(filePath, 'utf8');
    }
    catch (error) {
        if (ignoreMissing) {
            return null;
        }
        throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, `Failed to read credentials from file ${filePath}: ` + error);
    }
    try {
        return JSON.parse(fileText);
    }
    catch (error) {
        throw new error_1.FirebaseAppError(error_1.AppErrorCodes.INVALID_CREDENTIAL, 'Failed to parse contents of the credentials file as an object: ' + error);
    }
}