namespace EnterpriseBot.VK.Utils
{
    public static class ExceptionTemplates
    {
        public const string CriticalErrorSavedTemplate = "Ой! Что-то пошло не так.\n" +
                                                         "Код ошибки: {0}\n" +
                                                         "\n" +
                                                         "Если ошибка будет продолжаться, отправьте её код в техподдержку: {1}";

        public const string CriticalErrorSaveFailedTemplate = "Что-то пошло не так.\n" +
                                                              "\n" +
                                                              "Пожалуйста, обратитесь в техподдержку: {0}";

        public const string InvalidMessagePayloadWarnTemplate = "Пожалуйста, не пытайтесь менять payload сообщения. Это может привести к блокировке игрового аккаунта.\n" +
                                                                "\n" +
                                                                "Если произошла ошибка, Вы можете обратиться в техподдержку: {0}";

        public const string KeyboardNotSupportedTemplate = "Кажется, Ваш клиент не поддерживает клавиатуру. Если это не так, напишите в техподдержку: {0}";

        //public const string AccountAccessViolationMessageTemplate = "Пожалуйста, не пытайтесь получить доступ к чужому аккаунту. Это может привести к блокировке Вашего аккаунта.\n" +
        //                                                            "\n" +
        //                                                            "Если произошла ошибка, Вы можете обратиться в техподдержку: {0}";

        public const string StringNullOrEmptyTemplate = "{0} should not be null, empty or consist exclusively of white-space characters";

        public const string MethodDoesNotExistInTypeTemplate = "Method {0} does not exist in {1}";
    }
}
