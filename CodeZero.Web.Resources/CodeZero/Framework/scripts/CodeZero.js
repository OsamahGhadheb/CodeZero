﻿var CodeZero = CodeZero || {};
(function ($) {

    /* Application paths *****************************************/

    //Current application root path (including virtual directory if exists).
    CodeZero.appPath = CodeZero.appPath || '/';
    CodeZero.pageLoadTime = new Date();

    //Converts given path to absolute path using CodeZero.appPath variable.
    CodeZero.toAbsAppPath = function (path) {
        if (path.indexOf('/') == 0) {
            path = path.substring(1);
        }

        return CodeZero.appPath + path;
    };

    /* MULTITENANCY */

    CodeZero.multiTenancy = CodeZero.multiTenancy || {};

    CodeZero.multiTenancy.isEnabled = false;

    CodeZero.multiTenancy.sides = {
        TENANT: 1,
        HOST: 2
    };

    CodeZero.multiTenancy.tenantIdCookieName = 'CodeZero.TenantId';

    CodeZero.multiTenancy.setTenantIdCookie = function (tenantId) {
        if (tenantId) {
            CodeZero.utils.setCookieValue(
                CodeZero.multiTenancy.tenantIdCookieName,
                tenantId.toString(),
                new Date(new Date().getTime() + 5 * 365 * 86400000), //5 years
                CodeZero.appPath,
                CodeZero.domain
            );
        } else {
            CodeZero.utils.deleteCookie(CodeZero.multiTenancy.tenantIdCookieName, CodeZero.appPath);
        }
    };

    CodeZero.multiTenancy.getTenantIdCookie = function () {
        var value = CodeZero.utils.getCookieValue(CodeZero.multiTenancy.tenantIdCookieName);
        if (!value) {
            return null;
        }

        return parseInt(value);
    }

    /* SESSION */

    CodeZero.session = CodeZero.session ||
        {
            multiTenancySide: CodeZero.multiTenancy.sides.HOST
        };

    /* LOCALIZATION ***********************************************/
    //Implements Localization API that simplifies usage of localization scripts generated by CodeZero.

    CodeZero.localization = CodeZero.localization || {};

    CodeZero.localization.languages = [];

    CodeZero.localization.currentLanguage = {};

    CodeZero.localization.sources = [];

    CodeZero.localization.values = {};

    CodeZero.localization.localize = function (key, sourceName) {
        sourceName = sourceName || CodeZero.localization.defaultSourceName;

        var source = CodeZero.localization.values[sourceName];

        if (!source) {
            CodeZero.log.warn('Could not find localization source: ' + sourceName);
            return key;
        }

        var value = source[key];
        if (value == undefined) {
            return key;
        }

        var copiedArguments = Array.prototype.slice.call(arguments, 0);
        copiedArguments.splice(1, 1);
        copiedArguments[0] = value;

        return CodeZero.utils.formatString.apply(this, copiedArguments);
    };

    CodeZero.localization.getSource = function (sourceName) {
        return function (key) {
            var copiedArguments = Array.prototype.slice.call(arguments, 0);
            copiedArguments.splice(1, 0, sourceName);
            return CodeZero.localization.localize.apply(this, copiedArguments);
        };
    };

    CodeZero.localization.isCurrentCulture = function (name) {
        return CodeZero.localization.currentCulture
            && CodeZero.localization.currentCulture.name
            && CodeZero.localization.currentCulture.name.indexOf(name) == 0;
    };

    CodeZero.localization.defaultSourceName = undefined;
    CodeZero.localization.CodeZeroWeb = CodeZero.localization.getSource('CodeZeroWeb');

    /* AUTHORIZATION **********************************************/
    //Implements Authorization API that simplifies usage of authorization scripts generated by CodeZero.

    CodeZero.auth = CodeZero.auth || {};

    CodeZero.auth.allPermissions = CodeZero.auth.allPermissions || {};

    CodeZero.auth.grantedPermissions = CodeZero.auth.grantedPermissions || {};

    //Deprecated. Use CodeZero.auth.isGranted instead.
    CodeZero.auth.hasPermission = function (permissionName) {
        return CodeZero.auth.isGranted.apply(this, arguments);
    };

    //Deprecated. Use CodeZero.auth.isAnyGranted instead.
    CodeZero.auth.hasAnyOfPermissions = function () {
        return CodeZero.auth.isAnyGranted.apply(this, arguments);
    };

    //Deprecated. Use CodeZero.auth.areAllGranted instead.
    CodeZero.auth.hasAllOfPermissions = function () {
        return CodeZero.auth.areAllGranted.apply(this, arguments);
    };

    CodeZero.auth.isGranted = function (permissionName) {
        return CodeZero.auth.allPermissions[permissionName] != undefined && CodeZero.auth.grantedPermissions[permissionName] != undefined;
    };

    CodeZero.auth.isAnyGranted = function () {
        if (!arguments || arguments.length <= 0) {
            return true;
        }

        for (var i = 0; i < arguments.length; i++) {
            if (CodeZero.auth.isGranted(arguments[i])) {
                return true;
            }
        }

        return false;
    };

    CodeZero.auth.areAllGranted = function () {
        if (!arguments || arguments.length <= 0) {
            return true;
        }

        for (var i = 0; i < arguments.length; i++) {
            if (!CodeZero.auth.isGranted(arguments[i])) {
                return false;
            }
        }

        return true;
    };

    CodeZero.auth.tokenCookieName = 'CodeZero.AuthToken';

    CodeZero.auth.setToken = function (authToken, expireDate) {
        CodeZero.utils.setCookieValue(CodeZero.auth.tokenCookieName, authToken, expireDate, CodeZero.appPath, CodeZero.domain);
    };

    CodeZero.auth.getToken = function () {
        return CodeZero.utils.getCookieValue(CodeZero.auth.tokenCookieName);
    }

    CodeZero.auth.clearToken = function () {
        CodeZero.auth.setToken();
    }

    /* FEATURE SYSTEM *********************************************/
    //Implements Features API that simplifies usage of feature scripts generated by CodeZero.

    CodeZero.features = CodeZero.features || {};

    CodeZero.features.allFeatures = CodeZero.features.allFeatures || {};

    CodeZero.features.get = function (name) {
        return CodeZero.features.allFeatures[name];
    }

    CodeZero.features.getValue = function (name) {
        var feature = CodeZero.features.get(name);
        if (feature == undefined) {
            return undefined;
        }

        return feature.value;
    }

    CodeZero.features.isEnabled = function (name) {
        var value = CodeZero.features.getValue(name);
        return value == 'true' || value == 'True';
    }

    /* SETTINGS **************************************************/
    //Implements Settings API that simplifies usage of setting scripts generated by CodeZero.

    CodeZero.setting = CodeZero.setting || {};

    CodeZero.setting.values = CodeZero.setting.values || {};

    CodeZero.setting.get = function (name) {
        return CodeZero.setting.values[name];
    };

    CodeZero.setting.getBoolean = function (name) {
        var value = CodeZero.setting.get(name);
        return value == 'true' || value == 'True';
    };

    CodeZero.setting.getInt = function (name) {
        return parseInt(CodeZero.setting.values[name]);
    };

    /* REALTIME NOTIFICATIONS ************************************/

    CodeZero.notifications = CodeZero.notifications || {};

    CodeZero.notifications.severity = {
        INFO: 0,
        SUCCESS: 1,
        WARN: 2,
        ERROR: 3,
        FATAL: 4
    };

    CodeZero.notifications.userNotificationState = {
        UNREAD: 0,
        READ: 1
    };

    CodeZero.notifications.getUserNotificationStateAsString = function (userNotificationState) {
        switch (userNotificationState) {
            case CodeZero.notifications.userNotificationState.READ:
                return 'READ';
            case CodeZero.notifications.userNotificationState.UNREAD:
                return 'UNREAD';
            default:
                CodeZero.log.warn('Unknown user notification state value: ' + userNotificationState)
                return '?';
        }
    };

    CodeZero.notifications.getUiNotifyFuncBySeverity = function (severity) {
        switch (severity) {
            case CodeZero.notifications.severity.SUCCESS:
                return CodeZero.notify.success;
            case CodeZero.notifications.severity.WARN:
                return CodeZero.notify.warn;
            case CodeZero.notifications.severity.ERROR:
                return CodeZero.notify.error;
            case CodeZero.notifications.severity.FATAL:
                return CodeZero.notify.error;
            case CodeZero.notifications.severity.INFO:
            default:
                return CodeZero.notify.info;
        }
    };

    CodeZero.notifications.messageFormatters = {};

    CodeZero.notifications.messageFormatters['CodeZero.Notifications.MessageNotificationData'] = function (userNotification) {
        return userNotification.notification.data.message || userNotification.notification.data.properties.Message;
    };

    CodeZero.notifications.messageFormatters['CodeZero.Notifications.LocalizableMessageNotificationData'] = function (userNotification) {
        var message = userNotification.notification.data.message || userNotification.notification.data.properties.Message;
        var localizedMessage = CodeZero.localization.localize(
            message.name,
            message.sourceName
        );

        if (userNotification.notification.data.properties) {
            if ($) {
                //Prefer to use jQuery if possible
                $.each(userNotification.notification.data.properties, function (key, value) {
                    localizedMessage = localizedMessage.replace('{' + key + '}', value);
                });
            } else {
                //alternative for $.each
                var properties = Object.keys(userNotification.notification.data.properties);
                for (var i = 0; i < properties.length; i++) {
                    localizedMessage = localizedMessage.replace('{' + properties[i] + '}', userNotification.notification.data.properties[properties[i]]);
                }
            }
        }

        return localizedMessage;
    };

    CodeZero.notifications.getFormattedMessageFromUserNotification = function (userNotification) {
        var formatter = CodeZero.notifications.messageFormatters[userNotification.notification.data.type];
        if (!formatter) {
            CodeZero.log.warn('No message formatter defined for given data type: ' + userNotification.notification.data.type)
            return '?';
        }

        if (!CodeZero.utils.isFunction(formatter)) {
            CodeZero.log.warn('Message formatter should be a function! It is invalid for data type: ' + userNotification.notification.data.type)
            return '?';
        }

        return formatter(userNotification);
    }

    CodeZero.notifications.showUiNotifyForUserNotification = function (userNotification, options) {
        var message = CodeZero.notifications.getFormattedMessageFromUserNotification(userNotification);
        var uiNotifyFunc = CodeZero.notifications.getUiNotifyFuncBySeverity(userNotification.notification.severity);
        uiNotifyFunc(message, undefined, options);
    }

    /* LOGGING ***************************************************/
    //Implements Logging API that provides secure & controlled usage of console.log

    CodeZero.log = CodeZero.log || {};

    CodeZero.log.levels = {
        DEBUG: 1,
        INFO: 2,
        WARN: 3,
        ERROR: 4,
        FATAL: 5
    };

    CodeZero.log.level = CodeZero.log.levels.DEBUG;

    CodeZero.log.log = function (logObject, logLevel) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < CodeZero.log.level) {
            return;
        }

        console.log(logObject);
    };

    CodeZero.log.debug = function (logObject) {
        CodeZero.log.log("DEBUG: ", CodeZero.log.levels.DEBUG);
        CodeZero.log.log(logObject, CodeZero.log.levels.DEBUG);
    };

    CodeZero.log.info = function (logObject) {
        CodeZero.log.log("INFO: ", CodeZero.log.levels.INFO);
        CodeZero.log.log(logObject, CodeZero.log.levels.INFO);
    };

    CodeZero.log.warn = function (logObject) {
        CodeZero.log.log("WARN: ", CodeZero.log.levels.WARN);
        CodeZero.log.log(logObject, CodeZero.log.levels.WARN);
    };

    CodeZero.log.error = function (logObject) {
        CodeZero.log.log("ERROR: ", CodeZero.log.levels.ERROR);
        CodeZero.log.log(logObject, CodeZero.log.levels.ERROR);
    };

    CodeZero.log.fatal = function (logObject) {
        CodeZero.log.log("FATAL: ", CodeZero.log.levels.FATAL);
        CodeZero.log.log(logObject, CodeZero.log.levels.FATAL);
    };

    /* NOTIFICATION *********************************************/
    //Defines Notification API, not implements it

    CodeZero.notify = CodeZero.notify || {};

    CodeZero.notify.success = function (message, title, options) {
        CodeZero.log.warn('CodeZero.notify.success is not implemented!');
    };

    CodeZero.notify.info = function (message, title, options) {
        CodeZero.log.warn('CodeZero.notify.info is not implemented!');
    };

    CodeZero.notify.warn = function (message, title, options) {
        CodeZero.log.warn('CodeZero.notify.warn is not implemented!');
    };

    CodeZero.notify.error = function (message, title, options) {
        CodeZero.log.warn('CodeZero.notify.error is not implemented!');
    };

    /* MESSAGE **************************************************/
    //Defines Message API, not implements it

    CodeZero.message = CodeZero.message || {};

    var showMessage = function (message, title) {
        alert((title || '') + ' ' + message);

        if (!$) {
            CodeZero.log.warn('CodeZero.message can not return promise since jQuery is not defined!');
            return null;
        }

        return $.Deferred(function ($dfd) {
            $dfd.resolve();
        });
    };

    CodeZero.message.info = function (message, title) {
        CodeZero.log.warn('CodeZero.message.info is not implemented!');
        return showMessage(message, title);
    };

    CodeZero.message.success = function (message, title) {
        CodeZero.log.warn('CodeZero.message.success is not implemented!');
        return showMessage(message, title);
    };

    CodeZero.message.warn = function (message, title) {
        CodeZero.log.warn('CodeZero.message.warn is not implemented!');
        return showMessage(message, title);
    };

    CodeZero.message.error = function (message, title) {
        CodeZero.log.warn('CodeZero.message.error is not implemented!');
        return showMessage(message, title);
    };

    CodeZero.message.confirm = function (message, titleOrCallback, callback) {
        CodeZero.log.warn('CodeZero.message.confirm is not implemented!');

        if (titleOrCallback && !(typeof titleOrCallback == 'string')) {
            callback = titleOrCallback;
        }

        var result = confirm(message);
        callback && callback(result);

        if (!$) {
            CodeZero.log.warn('CodeZero.message can not return promise since jQuery is not defined!');
            return null;
        }

        return $.Deferred(function ($dfd) {
            $dfd.resolve();
        });
    };

    /* UI *******************************************************/

    CodeZero.ui = CodeZero.ui || {};

    /* UI BLOCK */
    //Defines UI Block API, not implements it

    CodeZero.ui.block = function (elm) {
        CodeZero.log.warn('CodeZero.ui.block is not implemented!');
    };

    CodeZero.ui.unblock = function (elm) {
        CodeZero.log.warn('CodeZero.ui.unblock is not implemented!');
    };

    /* UI BUSY */
    //Defines UI Busy API, not implements it

    CodeZero.ui.setBusy = function (elm, optionsOrPromise) {
        CodeZero.log.warn('CodeZero.ui.setBusy is not implemented!');
    };

    CodeZero.ui.clearBusy = function (elm) {
        CodeZero.log.warn('CodeZero.ui.clearBusy is not implemented!');
    };

    /* SIMPLE EVENT BUS *****************************************/

    CodeZero.event = (function () {

        var _callbacks = {};

        var on = function (eventName, callback) {
            if (!_callbacks[eventName]) {
                _callbacks[eventName] = [];
            }

            _callbacks[eventName].push(callback);
        };

        var off = function (eventName, callback) {
            var callbacks = _callbacks[eventName];
            if (!callbacks) {
                return;
            }

            var index = -1;
            for (var i = 0; i < callbacks.length; i++) {
                if (callbacks[i] === callback) {
                    index = i;
                    break;
                }
            }

            if (index < 0) {
                return;
            }

            _callbacks[eventName].splice(index, 1);
        };

        var trigger = function (eventName) {
            var callbacks = _callbacks[eventName];
            if (!callbacks || !callbacks.length) {
                return;
            }

            var args = Array.prototype.slice.call(arguments, 1);
            for (var i = 0; i < callbacks.length; i++) {
                callbacks[i].apply(this, args);
            }
        };

        // Public interface ///////////////////////////////////////////////////

        return {
            on: on,
            off: off,
            trigger: trigger
        };
    })();


    /* UTILS ***************************************************/

    CodeZero.utils = CodeZero.utils || {};

    /* Creates a name namespace.
    *  Example:
    *  var taskService = CodeZero.utils.createNamespace(CodeZero, 'services.task');
    *  taskService will be equal to CodeZero.services.task
    *  first argument (root) must be defined first
    ************************************************************/
    CodeZero.utils.createNamespace = function (root, ns) {
        var parts = ns.split('.');
        for (var i = 0; i < parts.length; i++) {
            if (typeof root[parts[i]] == 'undefined') {
                root[parts[i]] = {};
            }

            root = root[parts[i]];
        }

        return root;
    };

    /* Find and replaces a string (search) to another string (replacement) in
    *  given string (str).
    *  Example:
    *  CodeZero.utils.replaceAll('This is a test string', 'is', 'X') = 'ThX X a test string'
    ************************************************************/
    CodeZero.utils.replaceAll = function (str, search, replacement) {
        var fix = search.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
        return str.replace(new RegExp(fix, 'g'), replacement);
    };

    /* Formats a string just like string.format in C#.
    *  Example:
    *  CodeZero.utils.formatString('Hello {0}','Tuana') = 'Hello Tuana'
    ************************************************************/
    CodeZero.utils.formatString = function () {
        if (arguments.length < 1) {
            return null;
        }

        var str = arguments[0];

        for (var i = 1; i < arguments.length; i++) {
            var placeHolder = '{' + (i - 1) + '}';
            str = CodeZero.utils.replaceAll(str, placeHolder, arguments[i]);
        }

        return str;
    };

    CodeZero.utils.toPascalCase = function (str) {
        if (!str || !str.length) {
            return str;
        }

        if (str.length === 1) {
            return str.charAt(0).toUpperCase();
        }

        return str.charAt(0).toUpperCase() + str.substr(1);
    }

    CodeZero.utils.toCamelCase = function (str) {
        if (!str || !str.length) {
            return str;
        }

        if (str.length === 1) {
            return str.charAt(0).toLowerCase();
        }

        return str.charAt(0).toLowerCase() + str.substr(1);
    }

    CodeZero.utils.truncateString = function (str, maxLength) {
        if (!str || !str.length || str.length <= maxLength) {
            return str;
        }

        return str.substr(0, maxLength);
    };

    CodeZero.utils.truncateStringWithPostfix = function (str, maxLength, postfix) {
        postfix = postfix || '...';

        if (!str || !str.length || str.length <= maxLength) {
            return str;
        }

        if (maxLength <= postfix.length) {
            return postfix.substr(0, maxLength);
        }

        return str.substr(0, maxLength - postfix.length) + postfix;
    };

    CodeZero.utils.isFunction = function (obj) {
        if ($) {
            //Prefer to use jQuery if possible
            return $.isFunction(obj);
        }

        //alternative for $.isFunction
        return !!(obj && obj.constructor && obj.call && obj.apply);
    };

    /**
     * parameterInfos should be an array of { name, value } objects
     * where name is query string parameter name and value is it's value.
     * includeQuestionMark is true by default.
     */
    CodeZero.utils.buildQueryString = function (parameterInfos, includeQuestionMark) {
        if (includeQuestionMark === undefined) {
            includeQuestionMark = true;
        }


        var qs = '';

        function addSeperator() {
            if (!qs.length) {
                if (includeQuestionMark) {
                    qs = qs + '?';
                }
            } else {
                qs = qs + '&';
            }
        }

        for (var i = 0; i < parameterInfos.length; ++i) {
            var parameterInfo = parameterInfos[i];
            if (parameterInfo.value === undefined) {
                continue;
            }

            if (parameterInfo.value === null) {
                parameterInfo.value = '';
            }

            addSeperator();

            if (parameterInfo.value.toJSON && typeof parameterInfo.value.toJSON === "function") {
                qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value.toJSON());
            } else if (Array.isArray(parameterInfo.value) && parameterInfo.value.length) {
                for (var j = 0; j < parameterInfo.value.length; j++) {
                    if (j > 0) {
                        addSeperator();
                    }

                    qs = qs + parameterInfo.name + '[' + j + ']=' + encodeURIComponent(parameterInfo.value[j]);
                }
            } else {
                qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value);
            }
        }

        return qs;
    }

    /**
     * Sets a cookie value for given key.
     * This is a simple implementation created to be used by CodeZero.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @param {string} value 
     * @param {Date} expireDate (optional). If not specified the cookie will expire at the end of session.
     * @param {string} path (optional)
     */
    CodeZero.utils.setCookieValue = function (key, value, expireDate, path, domain) {
        var cookieValue = encodeURIComponent(key) + '=';

        if (value) {
            cookieValue = cookieValue + encodeURIComponent(value);
        }

        if (expireDate) {
            cookieValue = cookieValue + "; expires=" + expireDate.toUTCString();
        }

        if (path) {
            cookieValue = cookieValue + "; path=" + path;
        }

        if (domain) {
            cookieValue = cookieValue + "; domain=" + domain;
        }

        document.cookie = cookieValue;
    };

    /**
     * Gets a cookie with given key.
     * This is a simple implementation created to be used by CodeZero.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @returns {string} Cookie value or null
     */
    CodeZero.utils.getCookieValue = function (key) {
        var equalities = document.cookie.split('; ');
        for (var i = 0; i < equalities.length; i++) {
            if (!equalities[i]) {
                continue;
            }

            var splitted = equalities[i].split('=');
            if (splitted.length != 2) {
                continue;
            }

            if (decodeURIComponent(splitted[0]) === key) {
                return decodeURIComponent(splitted[1] || '');
            }
        }

        return null;
    };

    /**
     * Deletes cookie for given key.
     * This is a simple implementation created to be used by CodeZero.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @param {string} path (optional)
     */
    CodeZero.utils.deleteCookie = function (key, path) {
        var cookieValue = encodeURIComponent(key) + '=';

        cookieValue = cookieValue + "; expires=" + (new Date(new Date().getTime() - 86400000)).toUTCString();

        if (path) {
            cookieValue = cookieValue + "; path=" + path;
        }

        document.cookie = cookieValue;
    }

    /**
     * Gets the domain of given url
     * @param {string} url 
     * @returns {string} 
     */
    CodeZero.utils.getDomain = function (url) {
        var domainRegex = /(https?:){0,1}\/\/((?:[\w\d-]+\.)+[\w\d]{2,})/i;
        var matches = domainRegex.exec(url);
        return (matches && matches[2]) ? matches[2] : '';
    }

    /* TIMING *****************************************/
    CodeZero.timing = CodeZero.timing || {};

    CodeZero.timing.utcClockProvider = (function () {

        var toUtc = function (date) {
            return Date.UTC(
                date.getUTCFullYear()
                , date.getUTCMonth()
                , date.getUTCDate()
                , date.getUTCHours()
                , date.getUTCMinutes()
                , date.getUTCSeconds()
                , date.getUTCMilliseconds()
            );
        }

        var now = function () {
            return new Date();
        };

        var normalize = function (date) {
            if (!date) {
                return date;
            }

            return new Date(toUtc(date));
        };

        // Public interface ///////////////////////////////////////////////////

        return {
            now: now,
            normalize: normalize,
            supportsMultipleTimezone: true
        };
    })();

    CodeZero.timing.localClockProvider = (function () {

        var toLocal = function (date) {
            return new Date(
                date.getFullYear()
                , date.getMonth()
                , date.getDate()
                , date.getHours()
                , date.getMinutes()
                , date.getSeconds()
                , date.getMilliseconds()
            );
        }

        var now = function () {
            return toLocal(new Date());
        }

        var normalize = function (date) {
            if (!date) {
                return date;
            }

            return toLocal(date);
        }

        // Public interface ///////////////////////////////////////////////////

        return {
            now: now,
            normalize: normalize,
            supportsMultipleTimezone: false
        };
    })();

    CodeZero.timing.unspecifiedClockProvider = (function () {

        var now = function () {
            return new Date();
        }

        var normalize = function (date) {
            return date;
        }

        // Public interface ///////////////////////////////////////////////////

        return {
            now: now,
            normalize: normalize,
            supportsMultipleTimezone: false
        };
    })();

    CodeZero.timing.convertToUserTimezone = function (date) {
        var localTime = date.getTime();
        var utcTime = localTime + (date.getTimezoneOffset() * 60000);
        var targetTime = parseInt(utcTime) + parseInt(CodeZero.timing.timeZoneInfo.windows.currentUtcOffsetInMilliseconds);
        return new Date(targetTime);
    };

    /* CLOCK *****************************************/
    CodeZero.clock = CodeZero.clock || {};

    CodeZero.clock.now = function () {
        if (CodeZero.clock.provider) {
            return CodeZero.clock.provider.now();
        }

        return new Date();
    }

    CodeZero.clock.normalize = function (date) {
        if (CodeZero.clock.provider) {
            return CodeZero.clock.provider.normalize(date);
        }

        return date;
    }

    CodeZero.clock.provider = CodeZero.timing.unspecifiedClockProvider;

    /* SECURITY ***************************************/
    CodeZero.security = CodeZero.security || {};
    CodeZero.security.antiForgery = CodeZero.security.antiForgery || {};

    CodeZero.security.antiForgery.tokenCookieName = 'XSRF-TOKEN';
    CodeZero.security.antiForgery.tokenHeaderName = 'X-XSRF-TOKEN';

    CodeZero.security.antiForgery.getToken = function () {
        return CodeZero.utils.getCookieValue(CodeZero.security.antiForgery.tokenCookieName);
    };

    CodeZero.security.antiForgery.shouldSendToken = function (settings) {
        if (settings.crossDomain === undefined || settings.crossDomain === null) {
            return CodeZero.utils.getDomain(location.href) === CodeZero.utils.getDomain(settings.url);
        }

        return !settings.crossDomain;
    };

})(jQuery);