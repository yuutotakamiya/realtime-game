<?php

namespace Database\Seeders;

use App\Models\UserData;
use App\Models\UserMail;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class UserMailsTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        //
        UserMail::create([
            'user_id' => 1,
            'mail_id' => 1,
            'condition' => 0
        ]);
    }
}
