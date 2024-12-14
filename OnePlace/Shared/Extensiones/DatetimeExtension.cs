using System;

namespace OnePlace.Shared.Extenciones
{
    public static class DatetimeExtension
    {
        public static bool Between(this DateTime dateTime, DateTime DateStart, DateTime DateFinish) => dateTime >= DateStart && dateTime <= DateFinish;

        public static bool Between(this DateTime? dateTime, DateTime DateStart, DateTime DateFinish) => dateTime is not null && dateTime >= DateStart && dateTime <= DateFinish;
    }
}
