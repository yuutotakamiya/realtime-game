<?php

namespace Database\Seeders;

use App\Http\Controllers\UserListController;
use App\Models\Account;
use App\Models\User;

// use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     */
    public function run(): void
    {
        $this->call(AccountsTableSeeder::class);
        $this->call(ItemsTableSeeder::class);
        $this->call(UsersTableSeeder::class);
        $this->call(User_itemsTableSeeder::class);
        $this->call(MailsTableSeeder::class);
        $this->call(UserMailsTableSeeder::class);
        $this->call(FollowTableSeeder::class);
        $this->call(follow_logsTableSeeder::class);
        $this->call(StageTableSeeder::class);
        $this->call(StageLogTableSeeder::class);
        $this->call(LandsTableSeeder::class);
        $this->call(blockTableSeeder::class);
        $this->call(LandStatusTableSeeder::class);
    }
}
