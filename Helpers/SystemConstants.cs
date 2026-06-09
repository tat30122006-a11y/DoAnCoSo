namespace DoAnCoSo.Helpers
{
    public static class SystemConstants
    {
        public static class OrderStatus
        {
            public const string Pending = "chờ duyệt";
            public const string InTransit = "đang chuyển";
            public const string Received = "đã nhận";
            public const string Cancelled = "đã hủy";

            public const string InboundPending = "chờ nhập";
            public const string InboundCompleted = "đã nhập";

            public const string OutboundPending = "chờ xuất";
            public const string OutboundCompleted = "đã xuất";
        }

        public static class Roles
        {
            public const string Manager = "quản lý";
            public const string Staff = "nhân viên";
        }
    }
}