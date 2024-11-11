<?php

namespace Database\Seeders;

use App\Models\follow_logs;
use Illuminate\Database\Seeder;

class follow_logsTableSeeder extends Seeder
{
    public function run(): void
    {
        follow_logs::create([
            'user_id' => 1,
            'target_user_id' => 2,
            'action' => 0,
        ]);
        follow_logs::create([
            'user_id' => 2,
            'target_user_id' => 3,
            'action' => 1,
        ]);

    }
}
