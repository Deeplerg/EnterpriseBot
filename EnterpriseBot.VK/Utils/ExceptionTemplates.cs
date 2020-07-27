namespace EnterpriseBot.VK.Utils
{
    public static class ExceptionTemplates
    {
        public const string CriticalErrorSavedTemplate = "Ой! Что-то пошло не так.\n" +
                                                         "Код ошибки: {0}\n" +
                                                         "\n" +
                                                         "Пожалуйста, скопируй или перешли это сообщение в поддержку: {1}";

        public const string CriticalErrorSaveFailedTemplate = "Что-то пошло не так.\n" +
                                                              "\n" +
                                                              "Пожалуйста, обратись в поддержку: {0}";

        public const string InvalidMessagePayloadWarnTemplate = "Пожалуйста, не пытайся менять payload сообщения. Это может привести к блокировке игрового аккаунта.\n" +
                                                                "\n" +
                                                                "Если произошла ошибка, ты можешь обратиться в поддержку: {0}";

        public const string KeyboardNotSupportedTemplate = "Кажется, твой клиент не поддерживает клавиатуру. Если это не так, напиши в поддержку: {0}";

        //public const string AccountAccessViolationMessageTemplate = "Пожалуйста, не пытайся получить доступ к чужому аккаунту. Это может привести к блокировке.\n" +
        //                                                            "\n" +
        //                                                            "Если произошла ошибка, ты можешь обратиться в поддержку: {0}";

        public const string StringNullOrEmptyTemplate = "{0} should not be null, empty or consist exclusively of white-space characters";

        public const string MethodDoesNotExistInTypeTemplate = "Method {0} does not exist in {1}";
    }
}
