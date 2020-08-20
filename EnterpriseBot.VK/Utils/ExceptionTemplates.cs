namespace EnterpriseBot.VK.Utils
{
    public static class ExceptionTemplates
    {
        public const string CriticalErrorSavedTemplate = "Ой! Что-то пошло не так.\n" +
                                                         "Код ошибки: {0}\n" +
                                                         "\n" +
                                                         "Пожалуйста, скопируйте или перешлите это сообщение в поддержку: {1}";

        public const string CriticalErrorSaveFailedTemplate = "Что-то пошло не так.\n" +
                                                              "\n" +
                                                              "Пожалуйста, обратитесь в поддержку: {0}";

        public const string InvalidMessagePayloadWarnTemplate = "Похоже, что-то не так с сообщением, отправленным Вами.\n" +
                                                                "\n" +
                                                                "Если эта ошибка появилась у Вас, значит, она может появиться и у других игроков.\n" +
                                                                "Пожалуйста, обратитесь в поддержку как можно скорее: {0}";

        public const string KeyboardNotSupportedTemplate = "Кажется, Ваш клиент не поддерживает клавиатуру. Если это не так, напишите в поддержку: {0}";

        //public const string AccountAccessViolationMessageTemplate = "Пожалуйста, не пытайтесь получить доступ к чужому аккаунту. Это может привести к блокировке.\n" +
        //                                                            "\n" +
        //                                                            "Если произошла ошибка, Вы можете обратиться в поддержку: {0}";

        public const string StringNullOrEmptyTemplate = "{0} should not be null, empty or consist exclusively of white-space characters";

        public const string MethodDoesNotExistInTypeTemplate = "Method {0} does not exist in {1}";
    }
}
