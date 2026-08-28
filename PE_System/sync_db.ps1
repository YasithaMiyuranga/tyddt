Add-Type -Path "D:\work\stockara\PE_Stock_Management\PE_System\PE_System\bin\Debug\MySql.Data.dll"
$conn = New-Object MySql.Data.MySqlClient.MySqlConnection("server=localhost;userid=root;password=root;database=stock_management;Convert Zero Datetime=True")
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT cc.customer_id, cc.inv_no, cc.amount, cc.timestamps FROM customer_credit cc LEFT JOIN billing b ON cc.inv_no = b.inv_no WHERE b.inv_no IS NULL AND cc.is_active = 1 AND cc.inv_no IS NOT NULL AND cc.inv_no <> ''"
$reader = $cmd.ExecuteReader()
$dt = New-Object System.Data.DataTable
$dt.Load($reader)
$reader.Close()

Write-Host "Found missing billing for $($dt.Rows.Count) records."

$transaction = $conn.BeginTransaction()
try {
    foreach ($row in $dt.Rows) {
        $inv = $row["inv_no"]
        $cid = $row["customer_id"]
        $amt = $row["amount"]
        $ts = $row["timestamps"]
        
        Write-Host "Inserting dummy billing for inv: $inv, cid: $cid, amt: $amt"
        
        $insertCmd = $conn.CreateCommand()
        $insertCmd.Transaction = $transaction
        $insertCmd.CommandText = "INSERT IGNORE INTO billing (inv_no, printed_inv_no, customer_id, subtotal, grand_total, credit_balance_due, balance_due, status, timestamps, inv_type, billing_type, payment_type, user_id, order_user_id, collector_user_id, po_number) VALUES (@inv, @inv, @cid, @amount, @amount, @amount, @amount, 'Credit', @date, 'Manual Credit', 'Credit', 'Credit', 101, 101, 101, '')"
        
        $insertCmd.Parameters.AddWithValue("@inv", $inv) | Out-Null
        $insertCmd.Parameters.AddWithValue("@cid", $cid) | Out-Null
        $insertCmd.Parameters.AddWithValue("@amount", $amt) | Out-Null
        $insertCmd.Parameters.AddWithValue("@date", $ts) | Out-Null
        
        $insertCmd.ExecuteNonQuery() | Out-Null
    }
    $transaction.Commit()
    Write-Host "Transaction committed."
} catch {
    $transaction.Rollback()
    Write-Host "Transaction rolled back due to error: $($_.Exception.Message)"
}

$conn.Close()
