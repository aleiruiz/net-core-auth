﻿namespace XLocker.DTOs.CreditPackage
{
    public class UpdateCreditPackageDTO
    {
        public int CreditQuantity { get; set; }

        public float Price { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
